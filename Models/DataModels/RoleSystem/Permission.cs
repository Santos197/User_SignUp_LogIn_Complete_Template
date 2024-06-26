﻿using System.Collections.Generic;

namespace Models.DataModels.RoleSystem;

public class Permission : BaseModel
{
    public string Name { get; set; }
    public virtual ICollection<RolePermission> PermissionRoles { get; set; }
    public virtual ICollection<ActionPermission> PermissionAction { get; set; }
}