  /sg:workflow [Phase 2] Advanced UI & Connectivity

  Workflow Strategy: Systematic
  Persona: Architect (Primary), Frontend (DevExpress), Backend
  Context: Expanding the nU3.Framework with professional UI controls and real backend services.
  Goal: Integrate DevExpress components, implement the Service Agent Pattern, and establish real API communication.

  ---

  Implementation Roadmap

  Phase 2.1: DevExpress Base Framework Integration (The UI Standard)
  Objective: Replace standard WinForms controls with DevExpress equivalents and establish a theming system.
  MCP Context: DevExpress.XtraEditors, RibbonControl, GridControl, LayoutControl

   - [ ] Core UI Base Classes (`nU3.Core.UI`):
       - [ ] Update IWorkForm to support DevExpress interactions.
       - [ ] Create BaseXtraForm (inherits XtraForm) for Popups.
       - [ ] Create BaseWorkControl (inherits XtraUserControl) for MDI children.
       - [ ] Implement LayoutManager: Standardize margins, fonts, and control spacing via LayoutControl.

   - [ ] Shell Modernization (`nU3.Shell`):
       - [ ] Replace MenuStrip with RibbonControl.
       - [ ] Replace DocumentManager (Standard) with DevExpress DocumentManager (TabbedView).
       - [ ] Implement SkinManager: Allow users to switch DevExpress skins (Theming).
       - [ ] Implement SplashScreenManager: Show loading state during module loading.

   - [ ] Grid & Data Standard:
       - [ ] Create BaseGridControl: Pre-configured grid with sorting, filtering, and "Server Mode" support enabled
         by default.
       - [ ] Implement GridHelper: Utility to export to Excel/PDF.

  Phase 2.2: Service Agent Layer (The Connectivity)
  Objective: Abstract HTTP communication using Interface-based Clients (Refit/RestSharp).
  MCP Context: Refit, Newtonsoft.Json, Polly (Retry Policies)

   - [ ] Network Infrastructure (`nU3.Core.Network`):
       - [ ] Install Refit and Microsoft.Extensions.Http.
       - [ ] Implement AuthHeaderHandler: Automatically inject JWT into headers.
       - [ ] Implement GlobalExceptionHandler: Catch 401/500 errors and trigger UI alerts.

   - [ ] Service Interfaces:
       - [ ] Define IAuthService: Login, Logout, RefreshToken.
       - [ ] Define IPatientService: Search, GetDetails.
       - [ ] Define IDeploymentService: Replaces the local DB simulation for module checks.

   - [ ] Dependency Injection Setup:
       - [ ] Integrate Autofac or Microsoft.Extensions.DependencyInjection into nU3.Shell.
       - [ ] Register Service Agents (RestService.For<IAuthService>).

  Phase 2.3: Security & Session Management (The Guard)
  Objective: Implement real Login flow and User Context management.

   - [ ] Login Module (`nU3.Modules.Login`):
       - [ ] Create LoginForm (Dialog).
       - [ ] Integrate IAuthService to validate credentials.
       - [ ] On success, initialize UserSession singleton.

   - [ ] Session Context:
       - [ ] UserSession: Store UserId, Token, DepartmentId.
       - [ ] PatientContext: Global current patient state (for broadcasting).

  Phase 2.4: Event Aggregator (The Messenger)
  Objective: Enable communication between Forms without direct reference.
  MCP Context: Pub/Sub Pattern, System.Reactive or Custom EventBus

   - [ ] Event Bus Implementation:
       - [ ] Create IEventAggregator interface.
       - [ ] Implement EventAggregator using weak references (to prevent memory leaks).
       - [ ] Define Standard Events: PatientSelectedEvent, OrderSignedEvent.

   - [ ] Integration:
       - [ ] Inject IEventAggregator into BaseWorkControl.
       - [ ] Create demo: PatientSearchModule publishes event -> PatientDetailModule updates UI.

  ---

  Detailed Implementation Tasks (Next Steps)

  Task 2.1: DevExpress Base Classes
  Persona: Frontend Specialist
  Priority: Critical
  Description: Create the base classes that all future modules will inherit from.

  Task 2.2: Event Aggregator
  Persona: Architect
  Priority: High
  Description: Essential for decoupling modules.

  Task 2.3: Login & DI Container
  Persona: Backend/Architect
  Priority: High
  Description: Switch from manual instantiation to DI container for better manageability.

  ---
