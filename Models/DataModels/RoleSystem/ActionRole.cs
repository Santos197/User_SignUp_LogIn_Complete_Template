﻿namespace Models.DataModels.RoleSystem;

public class ActionRole : BaseModel
{
    public string ActionName { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
}