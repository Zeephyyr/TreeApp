namespace Repositories
{
    public class SqlQueries
    {
        public const string InsertFullNode = @"insert into NodeList(Id,Text,UserId,Value,
                                            ParentId,Generation,DateCreation) values
                                            (@Id,@Text,@UserId,@Value,@ParentId,@Generation,@DateCreation)";
        public const string InsertRootNode = @"insert into NodeList(Id,Text,UserId,Value,Generation,DateCreation) values
                                            (@Id,@Text,@UserId,@Value,@Generation,@DateCreation)";

        public const string GetTree = @"select Id,Text,UserId,Value,ParentId,Generation from NodeList
                                                where UserId=@UserId order by DateCreation";

        public const string RemoveNode = @"delete * from NodeList where Value=@Value and UserId=@UserId";

        public const string CheckNodeId = @"Select top 1 * from NodeList where Value=@Value and UserId=@UserId";

        public const string RemoveTree = @"Delete from NodeList where UserId=@UserId";

        public const string UpdateNode = @"update NodeList
                                         set Value=@Value, Text=@Text
                                         where Id=@NodeId";

        public const string GetNode = @"select Id,Text,UserId,Value,ParentId,Generation from NodeList
                                                where Id=@NodeId";

        public const string InitBasicTables = @"if not exists(select * from sysobjects where name='AspNetUsers' and xtype='U')
            CREATE TABLE  [dbo].[AspNetUsers](
	            [Id] [nvarchar](128) NOT NULL,
	            [Email] [nvarchar](256) NULL,
	            [EmailConfirmed] [bit] NOT NULL,
	            [PasswordHash] [nvarchar](max) NULL,
	            [SecurityStamp] [nvarchar](max) NULL,
	            [PhoneNumber] [nvarchar](max) NULL,
	            [PhoneNumberConfirmed] [bit] NOT NULL,
	            [TwoFactorEnabled] [bit] NOT NULL,
	            [LockoutEndDateUtc] [datetime] NULL,
	            [LockoutEnabled] [bit] NOT NULL,
	            [AccessFailedCount] [int] NOT NULL,
	            [UserName] [nvarchar](256) NOT NULL,
             CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

            if not exists(select * from sysobjects where name='AspNetRoles' and xtype='U')
            CREATE TABLE [dbo].[AspNetRoles](
	            [Id] [nvarchar](128) NOT NULL,
	            [Name] [nvarchar](256) NOT NULL,
             CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]

            if not exists(select * from sysobjects where name='AspNetUserRoles' and xtype='U')
            CREATE TABLE [dbo].[AspNetUserRoles](
	            [UserId] [nvarchar](128) NOT NULL,
	            [RoleId] [nvarchar](128) NOT NULL,
             CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
            (
	            [UserId] ASC,
	            [RoleId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]

            if not exists(select * from sysobjects where name='FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId')
            ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
            REFERENCES [dbo].[AspNetRoles] ([Id])
            ON DELETE CASCADE

            ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]

            if not exists(select * from sysobjects where name='FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId')
            ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id])
            ON DELETE CASCADE

            ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]

            if not exists(select * from sysobjects where name='AspNetUserClaims' and xtype='U')
            CREATE TABLE [dbo].[AspNetUserClaims](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [UserId] [nvarchar](128) NOT NULL,
	            [ClaimType] [nvarchar](max) NULL,
	            [ClaimValue] [nvarchar](max) NULL,
             CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

            if not exists(select * from sysobjects where name='FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId')
            ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id])
            ON DELETE CASCADE

            ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]

            if not exists(select * from sysobjects where name='AspNetUserLogins' and xtype='U')
            CREATE TABLE [dbo].[AspNetUserLogins](
	            [LoginProvider] [nvarchar](128) NOT NULL,
	            [ProviderKey] [nvarchar](128) NOT NULL,
	            [UserId] [nvarchar](128) NOT NULL,
             CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
            (
	            [LoginProvider] ASC,
	            [ProviderKey] ASC,
	            [UserId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]

            if not exists(select * from sysobjects where name='FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId')
            ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id])
            ON DELETE CASCADE

            ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]";

        public const string InitNodesTable = @"if not exists(select * from sysobjects where name='NodeList' and xtype='U')
                            Create table [dbo].[NodeList]
                            (
                            Id uniqueidentifier not null default newid(),
                            [Text] varchar(50) not null,
                            UserId nvarchar(128) NOT NULL,
                            [Value] integer not null,
                            ParentId varchar(128) null,
                            Generation integer not null,
                            DateCreation datetime not null

                            Constraint PK_NodeList_Id primary key clustered(Id),
                            CONSTRAINT FK_NodeList_Users FOREIGN KEY (UserId)
                            REFERENCES AspNetUsers(Id)
                            )";
    }
}
