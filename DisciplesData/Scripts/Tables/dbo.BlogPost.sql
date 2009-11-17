CREATE TABLE [dbo].[BlogPost]
(
[Id] [int] NOT NULL IDENTITY(568, 1),
[Title] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[BlogId] [int] NOT NULL,
[Post] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EntryDate] [datetime] NULL,
[Updated] [datetime] NULL,
[EnclosureUrl] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[EnclosureLength] [int] NULL,
[EnclosureType] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsPublic] [bit] NOT NULL CONSTRAINT [DF_BlogPost_IsPublic] DEFAULT ((0)),
[NotifyLater] [bit] NOT NULL CONSTRAINT [DF_BlogPost_NotifyLater] DEFAULT ((0)),
[PosterId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[BlogPost] ADD CONSTRAINT [PK_BlogPost] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BlogPost] ADD CONSTRAINT [FK_BlogPost_Blog] FOREIGN KEY ([BlogId]) REFERENCES [dbo].[Blog] ([Id])
GO
ALTER TABLE [dbo].[BlogPost] ADD CONSTRAINT [FK_BlogPost_Users] FOREIGN KEY ([PosterId]) REFERENCES [dbo].[Users] ([UserId])
GO