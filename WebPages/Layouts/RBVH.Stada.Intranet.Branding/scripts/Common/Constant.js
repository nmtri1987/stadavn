RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.Constants");
RBVH.Stada.WebPages.Constants = {
    DateTimeFormat: {
        VietNameseDateTime: 'DD/MM/YYYY HH:mm'
    },
    PageLimit: 20,

    EmployeeLevel:
    {
        BOD: 7,
        DepartmentHead: 5,
        GroupLeader: 4,
        Administrator: 3,
        TeamLeader: 3.2, //To Truong
        AssociateTeamLeader: 3.1, //To Pho
        ShiftLeader: 3.3,
        Employee: 2,
        Helper: 1,
        SecurityGuard: 1,
        Gardener: 1,
        DirectManagement: 6,
    },

    ApprovalStatus: {
        En_US: {
            InProgress: 'Approval',
            Approved: 'Approved',
            Cancelled: 'Cancelled',
            Rejected: 'Rejected',
            InProcess: 'In-Process',
            Completed: 'Completed'
        },
        Vi_VN: {
            InProgress: 'Approval',
            Approved: 'Đã duyệt',
            Cancelled: 'Đã hủy',
            Rejected: 'Từ chối',
            InProcess: 'Đang thực hiện',
            Completed: 'Hoàn thành'
        }
    }
};
