namespace Asgla.Request {
    public enum RequestType {
        Default = 0,

        Login = 1,

        PlayerUpdate = 2,
        Move = 3,

        JoinMap = 4,
        LeaveMap = 5,

        Chat = 6,

        EquipPart = 7,

        Notification = 8,
        Pong = 9,

        Experience = 10,

        MoveToArea = 11,

        PlayerDataLoad = 12,
        PlayerInventoryLoad = 13,
        AvatarDataUpdate = 14,

        AvatarCombat = 15,

        PlayerRespawn = 16,

        SkillLoad = 17,

        ShopLoad = 18,

        QuestLoad = 19,

        PlayerInventoryUpdate = 20,
        PlayerInventoryRemove = 21,
    }
}
