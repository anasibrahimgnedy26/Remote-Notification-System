ALTER TABLE Notifications ADD TargetType nvarchar(20) NOT NULL DEFAULT 'All';
ALTER TABLE Notifications ADD TargetValue nvarchar(500) NULL;
GO
