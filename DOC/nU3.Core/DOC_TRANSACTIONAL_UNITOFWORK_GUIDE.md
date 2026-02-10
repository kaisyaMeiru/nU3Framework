# TransactionalUnitOfWork 사용 가이드

## 개요

`TransactionalUnitOfWork`는 DB 작업과 파일 작업을 하나의 트랜잭션으로 묶어 처리합니다.
**Saga 패턴**을 적용하여 분산 트랜잭션을 관리합니다.

```
┌─────────────────────────────────────────────────────────────┐
│              TransactionalUnitOfWork                         │
├─────────────────────────────────────────────────────────────┤
│  Operation 1: INSERT INTO ORDERS...                          │
│       ↓ 성공                                                 │
│  Operation 2: Upload invoice.pdf                             │
│       ↓ 성공                                                 │
│  Operation 3: UPDATE INVENTORY...                            │
│       ↓ 실패 ?                                              │
├─────────────────────────────────────────────────────────────┤
│  자동 롤백 시작:                                              │
│       ← Operation 2: Delete invoice.pdf from server          │
│       ← Operation 1: DELETE FROM ORDERS WHERE...             │
└─────────────────────────────────────────────────────────────┘
```

---

## 기본 사용법

### 1. 순차 실행 (권장)

```csharp
using var unitOfWork = new TransactionalUnitOfWork();

// 1. DB INSERT
unitOfWork.AddNonQuery(
    operationId: "InsertOrder",
    commandText: "INSERT INTO ORDERS (ORDER_ID, CUSTOMER_ID, AMOUNT) VALUES (:orderId, :customerId, :amount)",
    parameters: new Dictionary<string, object>
    {
        { "orderId", orderId },
        { "customerId", customerId },
        { "amount", 10000 }
    },
    // 롤백 시 실행할 쿼리
    rollbackCommandText: "DELETE FROM ORDERS WHERE ORDER_ID = :orderId",
    rollbackParameters: new Dictionary<string, object> { { "orderId", orderId } }
);

// 2. 파일 업로드
unitOfWork.AddFileUpload(
    operationId: "UploadInvoice",
    localPath: @"C:\temp\invoice.pdf",
    serverPath: $"invoices/{orderId}/invoice.pdf"
);

// 3. DB UPDATE
unitOfWork.AddNonQuery(
    operationId: "UpdateInventory",
    commandText: "UPDATE INVENTORY SET QTY = QTY - :qty WHERE PRODUCT_ID = :productId",
    parameters: new Dictionary<string, object>
    {
        { "qty", 1 },
        { "productId", productId }
    },
    rollbackCommandText: "UPDATE INVENTORY SET QTY = QTY + :qty WHERE PRODUCT_ID = :productId",
    rollbackParameters: new Dictionary<string, object>
    {
        { "qty", 1 },
        { "productId", productId }
    }
);

// 실행
var progress = new Progress<UnitOfWorkProgressEventArgs>(p =>
{
    Console.WriteLine($"[{p.Phase}] {p.CurrentOperationId} ({p.PercentComplete}%)");
});

bool success = await unitOfWork.ExecuteAsync(progress);

if (success)
{
    Console.WriteLine("모든 작업 완료!");
}
else
{
    Console.WriteLine($"실패: {unitOfWork.ErrorMessage}");
    // 롤백은 자동으로 수행됨
}
```

### 2. 병렬 실행 (독립적인 작업들)

```csharp
using var unitOfWork = new TransactionalUnitOfWork();

// 여러 파일 업로드 (병렬 처리 가능)
for (int i = 0; i < files.Length; i++)
{
    unitOfWork.AddFileUpload(
        operationId: $"Upload_{i}",
        localPath: files[i].LocalPath,
        serverPath: files[i].ServerPath
    );
}

// 병렬 실행
bool success = await unitOfWork.ExecuteParallelAsync(progress);
```

---

## UI 연동 (WinForms)

```csharp
private async void btnSaveOrder_Click(object sender, EventArgs e)
{
    try
    {
        using var unitOfWork = new TransactionalUnitOfWork();
        
        // 작업 추가
        unitOfWork
            .AddNonQuery("InsertOrder", insertSql, insertParams, rollbackSql, rollbackParams)
            .AddFileUpload("UploadInvoice", localPath, serverPath)
            .AddNonQuery("UpdateStock", updateSql, updateParams, updateRollbackSql, updateRollbackParams);

        // 진행률 표시와 함께 실행
        bool success = await AsyncOperationHelper.ExecuteWithProgressAsync(
            this,
            "주문 처리 중...",
            async (cancellationToken, progress) =>
            {
                var unitOfWorkProgress = new Progress<UnitOfWorkProgressEventArgs>(p =>
                {
                    progress.Report(new BatchOperationProgress
                    {
                        TotalItems = p.TotalOperations,
                        CompletedItems = p.CurrentOperationIndex,
                        CurrentItem = $"{p.Phase}: {p.CurrentOperationId}",
                        PercentComplete = p.PercentComplete
                    });
                });
                
                return await unitOfWork.ExecuteAsync(unitOfWorkProgress, cancellationToken);
            },
            allowCancel: true);

        if (success)
        {
            XtraMessageBox.Show("주문이 완료되었습니다.", "성공");
        }
        else
        {
            XtraMessageBox.Show($"주문 실패: {unitOfWork.ErrorMessage}\n\n모든 변경사항이 롤백되었습니다.", "실패");
        }
    }
    catch (OperationCanceledException)
    {
        XtraMessageBox.Show("사용자가 작업을 취소했습니다.\n모든 변경사항이 롤백되었습니다.", "취소됨");
    }
}
```

---

## 고급 사용법

### 조건부 작업 추가

```csharp
unitOfWork
    .AddNonQuery("InsertOrder", insertSql, insertParams, rollbackSql, rollbackParams)
    .AddConditional(
        predicate: () => hasAttachment,  // 첨부파일이 있을 때만
        addOperations: uow => uow.AddFileUpload("UploadAttachment", localPath, serverPath)
    )
    .AddNonQuery("UpdateStatus", updateSql, updateParams, rollbackSql, rollbackParams);
```

### 쿼리 결과 사용

```csharp
System.Data.DataTable? orderData = null;

unitOfWork
    .AddQuery(
        "SelectOrder",
        "SELECT * FROM ORDERS WHERE ORDER_ID = :orderId",
        new Dictionary<string, object> { { "orderId", orderId } },
        onSuccess: data => orderData = data  // 결과 저장
    )
    .AddNonQuery(
        "UpdateOrder",
        "UPDATE ORDERS SET STATUS = 'PROCESSED' WHERE ORDER_ID = :orderId",
        new Dictionary<string, object> { { "orderId", orderId } },
        rollbackCommandText: "UPDATE ORDERS SET STATUS = 'PENDING' WHERE ORDER_ID = :orderId",
        rollbackParameters: new Dictionary<string, object> { { "orderId", orderId } }
    );

await unitOfWork.ExecuteAsync();

// orderData에 쿼리 결과가 저장됨
if (orderData != null)
{
    Console.WriteLine($"주문 수: {orderData.Rows.Count}");
}
```

### 커스텀 Operation 구현

```csharp
public class SendEmailOperation : ITransactionalOperation
{
    public string OperationId { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    
    private bool _sent;
    private string _messageId;

    public async Task<OperationResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 이메일 발송
            _messageId = await EmailService.SendAsync(To, Subject, Body);
            _sent = true;
            
            return new OperationResult { OperationId = OperationId, Success = true };
        }
        catch (Exception ex)
        {
            return new OperationResult { OperationId = OperationId, Success = false, Error = ex.Message };
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_sent)
        {
            // 발송 취소 또는 취소 메일 발송
            await EmailService.SendCancellationAsync(_messageId);
        }
    }
}

// 사용
unitOfWork.AddCustomOperation(new SendEmailOperation
{
    OperationId = "SendConfirmation",
    To = customer.Email,
    Subject = "주문 확인",
    Body = "주문이 접수되었습니다."
});
```

---

## 롤백 동작

| 작업 유형 | 롤백 동작 |
|----------|----------|
| DB INSERT | rollbackCommandText 실행 (DELETE) |
| DB UPDATE | rollbackCommandText 실행 (원래 값으로 복원) |
| DB DELETE | rollbackCommandText 실행 (INSERT) |
| File Upload | 서버에서 업로드된 파일 삭제 |
| File Download | 로컬에서 다운로드된 파일 삭제 |
| Custom | RollbackAsync() 메서드 호출 |

---

## 상태 확인

```csharp
// 실행 후 상태 확인
Console.WriteLine($"상태: {unitOfWork.State}");
// Pending, Executing, Completed, RollingBack, RolledBack, Failed

// 각 작업 결과 확인
foreach (var result in unitOfWork.Results)
{
    Console.WriteLine($"  {result.OperationId}: {(result.Success ? "성공" : $"실패 - {result.Error}")}");
}

// 에러 메시지
if (!string.IsNullOrEmpty(unitOfWork.ErrorMessage))
{
    Console.WriteLine($"에러: {unitOfWork.ErrorMessage}");
}
```

---

## 주의사항

1. **롤백 쿼리 필수**: DB 변경 작업(INSERT/UPDATE/DELETE)에는 반드시 rollbackCommandText를 지정하세요.

2. **멱등성 고려**: 롤백 쿼리는 여러 번 실행되어도 안전해야 합니다.

3. **순서 중요**: ExecuteAsync()는 추가한 순서대로 실행됩니다. 의존성이 있는 작업은 순서를 지켜야 합니다.

4. **병렬 실행 주의**: ExecuteParallelAsync()는 작업 간 의존성이 없을 때만 사용하세요.

5. **리소스 해제**: using 문 또는 Dispose()를 사용하여 리소스를 해제하세요.
