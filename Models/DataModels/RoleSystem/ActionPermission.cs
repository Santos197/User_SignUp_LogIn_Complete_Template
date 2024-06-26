﻿namespace Models.DataModels.RoleSystem;

public class ActionPermission : BaseModel
{
    public string ActionName { get; set; }
    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; }
}