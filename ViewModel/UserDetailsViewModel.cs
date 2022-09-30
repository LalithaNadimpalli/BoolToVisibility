using System.Collections.ObjectModel;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using BoolToVisibility.Contracts.ViewModels;
    using BoolToVisibility.Core.Contracts.Services;
    using BoolToVisibility.Enums;
    using BoolToVisibility.Helpers;
    using BoolToVisibility.Models;
    using Windows.Networking.Proximity;
    using Windows.System;

    namespace BoolToVisibility.ViewModel;

    public class UserDetailsViewModel : ObservableRecipient, IUserDetailsViewModel
    {
        private readonly IUserDataService _userDataService;
        private bool _isCreateNewUser;
        private bool _isEdit;
        private bool _isEditVisible;
        private bool _isMultiSelection;
        private bool _isReadOnly;
        private bool _isSingleSelection;
        private IList<object>? _selectedItems;
        private List<User>? _selectedUsers;
        private string? _selectedUserStatus;
        private User? _user;
        private string? _userDepartment;
        private ObservableCollection<string>? _userDepartments;
        private List<Department>? _userDepartmentsStore;
        private string? _userEmail;
        private List<UserFunction>? _userFunctions;
        private List<UserFunction>? _userFunctionsStore;
        private string? _userName;
        private List<UserRole>? _userRoles;
        private List<UserRole>? _userRolesStore;
        private ObservableCollection<string>? _userStatus;


        public UserDetailsViewModel(IUserDataService userDataService)
        {
            _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
            AddUserCommand = new RelayCommand(AddUserCommandExecute, CanAddUserCommandExecute);
            EditUserCommand = new RelayCommand(EditUserCommandExecute);
            SubmitCommand = new RelayCommand(SubmitCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            SelectedItemsCommand = new RelayCommand<object>(SelectedItemsCommandExecute);
            UserStatus = SetupUserStatus();
            SelectedUserStatus = EnumHelper.ConvertEnumToString(Enums.UserStatus.Active);
            UserDepartments = new ObservableCollection<string>();
            UserRoles = new List<UserRole>();
            UserFunctions = new List<UserFunction>();
            SelectedUsers = new List<User>();
        }
    public UserDetailsViewModel ViewModel
    {
        get;
    }

    public AddEditControl()
    {
        ViewModel = App.GetService<UserDetailsViewModel>();
        this.InitializeComponent();
    }

    public IRelayCommand AddUserCommand
        {
            get;
            set;
        }

        public IRelayCommand EditUserCommand
        {
            get;
            set;
        }

        public ICommand SubmitCommand
        {
            get;
            set;
        }

        public ICommand CancelCommand
        {
            get;
            set;
        }

        public ICommand SelectedItemsCommand
        {
            get;
            set;
        }

        public List<UserRole>? UserRoles
        {
            get => _userRoles;
            set => SetProperty(ref _userRoles, value);
        }

        public ObservableCollection<string>? UserDepartments
        {
            get => _userDepartments;
            set => SetProperty(ref _userDepartments, value);
        }

        public List<UserFunction>? UserFunctions
        {
            get => _userFunctions;
            set => SetProperty(ref _userFunctions, value);
        }

        public string? UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string? UserDepartment
        {
            get => _userDepartment;
            set => SetProperty(ref _userDepartment, value);
        }

        public string? UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public bool IsMultiSelection
        {
            get => _isMultiSelection;
            set => SetProperty(ref _isMultiSelection, value);
        }

        public string? SelectedUserStatus
        {
            get => _selectedUserStatus;
            set => SetProperty(ref _selectedUserStatus, value);
        }

        public ObservableCollection<string>? UserStatus
        {
            get => _userStatus;
            set => SetProperty(ref _userStatus, value);
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public bool IsEditVisible
        {
            get => _isEditVisible;
            set => SetProperty(ref _isEditVisible, value);
        }

        public bool IsCreateNewUser
        {
            get => _isCreateNewUser;
            set => SetProperty(ref _isCreateNewUser, value);
        }

        public List<UserRole> SelectedUserRoles
        {
            get;
            set;
        } = new();

        public List<Department> SelectedUserDepartments
        {
            get;
            set;
        } = new();

        public List<UserFunction> SelectedUserFunctions
        {
            get;
            set;
        } = new();

        public List<User>? SelectedUsers
        {
            get => _selectedUsers;
            set => SetProperty(ref _selectedUsers, value);
        }

        public bool IsSingleSelection
        {
            get => _isSingleSelection;
            set => SetProperty(ref _isSingleSelection, value);
        }

        private bool CanAddUserCommandExecute()
        {
            if (IsEdit)
            {
                return false;
            }

            return true;
        }

        private void CancelCommandExecute()
        {
            if (SelectedUsers?.Count == 0 && _user != null)
            {
                IsEdit = false;
                IsReadOnly = true;
                RefreshUserData();
            }
        }

        private static ObservableCollection<string> SetupUserStatus()
        {
            var setupUserStatus = Enum.GetNames(typeof(UserStatus)).Where(u => !u.Contains("All"));
            return new ObservableCollection<string>(setupUserStatus);
        }

        private void AddUserCommandExecute()
        {
            IsSingleSelection = false;
            IsMultiSelection = false;
            IsEdit = false;
            IsReadOnly = false;
            IsCreateNewUser = true;
            _user = new User();
            UserName = _user.UserName;
            UserEmail = _user.UserEmail;
            UserDepartment = _user.DepartmentName;
            SelectedUserStatus = EnumHelper.ConvertEnumToString(Enums.UserStatus.Active);
            EditUserCommand.NotifyCanExecuteChanged();
        }

        private void SubmitCommandExecute()
        {
        }

        private void EditUserCommandExecute()
        {
            if (!IsEdit)
            {
                IsReadOnly = true;
            }
            else
            {
                IsReadOnly = false;
            }
            RefreshUserData();
        }

        public void SetUser(User user)
        {
            _user = user;
            IsReadOnly = true;
            IsMultiSelection = false;
            IsCreateNewUser = false;
            IsEdit = false;
            IsEditVisible = true;
            RefreshUserData();
        }

        public void SetupUsers(IEnumerable<User> selectedUsers)
        {
            _user = null;
            IsSingleSelection = false;
            IsCreateNewUser = false;
            SelectedUsers?.Clear();
            SelectedUsers = selectedUsers.ToList();
            IsEditVisible = false;
        }

        private void RefreshUserData()
        {
            UserName = _user?.UserName;
            UserEmail = _user?.UserEmail;
            UserDepartment = _user?.DepartmentName;
            SelectedUserStatus = _user is { UserActiveIndicator: true }
                ? EnumHelper.ConvertEnumToString(Enums.UserStatus.Active)
                : EnumHelper.ConvertEnumToString(Enums.UserStatus.Inactive);
        }


        public void SetUpUserData(List<UserRole> userRolesStore, List<Department> userDepartmentsStore,
            List<UserFunction> userFunctionsStore)
        {
            _userDepartmentsStore = userDepartmentsStore;
            _userRolesStore = userRolesStore;
            _userFunctionsStore = userFunctionsStore;
            foreach (var department in _userDepartmentsStore)
            {
                UserDepartments?.Add(department.DepartmentName);
            }

            foreach (var userRole in _userRolesStore)
            {
                SelectedUserRoles.Add(userRole);
                UserRoles?.Add(userRole);
            }

            foreach (var userFunction in _userFunctionsStore)
            {
                SelectedUserFunctions.Add(userFunction);
                UserFunctions?.Add(userFunction);
            }

            IsEditVisible = false;
            IsCreateNewUser = false;
        }

        private void SelectedItemsCommandExecute(object? obj)
        {
            if (obj != null)
            {
                _selectedItems = obj as IList<object>;

                if (_selectedItems != null && _selectedItems.Any())
                {
                    ProcessSelectedFlyoutItems(_selectedItems);
                }
            }
        }

        private void ProcessSelectedFlyoutItems(IList<object>? selectedItems)
        {
            if (selectedItems != null)
            {
                foreach (var item in selectedItems)
                {
                    if (item is UserRole userRole)
                    {
                        if (!SelectedUserRoles.Contains(item))
                        {
                            SelectedUserRoles.Add(userRole);
                        }
                    }
                    else if (item is Department department)
                    {
                        if (!SelectedUserDepartments.Contains(item))
                        {
                            SelectedUserDepartments.Add(department);
                        }
                    }
                    else if (item is UserFunction userFunction)
                    {
                        if (!SelectedUserFunctions.Contains(item))
                        {
                            SelectedUserFunctions.Add(userFunction);
                        }
                    }
                }
            }
        }

        public void SetNewUser()
        {
            IsSingleSelection = false;
            IsMultiSelection = false;
            IsEdit = false;
            IsEditVisible = false;
            IsReadOnly = false;
            IsCreateNewUser = true;
            _user = new User();
            UserName = _user.UserName;
            UserEmail = _user.UserEmail;
            UserDepartment = _user.DepartmentName;
            SelectedUserStatus = EnumHelper.ConvertEnumToString(Enums.UserStatus.Active);
        }
    }
