USE Master
GO

CREATE DATABASE MemberDirectory;
GO

USE [MemberDirectory]
GO
/****** Object:  UserDefinedTableType [dbo].[IntValuesType]    Script Date: 8/3/2021 7:23:36 PM ******/
CREATE TYPE [dbo].[IntValuesType] AS TABLE(
	[Item] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[StringValuesType]    Script Date: 8/3/2021 7:23:36 PM ******/
CREATE TYPE [dbo].[StringValuesType] AS TABLE(
	[Item] [nvarchar](120) NULL
)
GO
/****** Object:  Table [dbo].[Friendship]    Script Date: 8/3/2021 7:23:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friendship](
	[MemberId] [int] NOT NULL,
	[FriendMemberId] [int] NOT NULL,
	[DateCreated] [datetime2](0) NULL,
 CONSTRAINT [PK_Friendship] PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC,
	[FriendMemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 8/3/2021 7:23:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberName] [nvarchar](80) NOT NULL,
	[WebsiteUrl] [nvarchar](300) NOT NULL,
	[WebsiteShortUrl] [nvarchar](50) NULL,
	[DateCreated] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Member_Unique] UNIQUE NONCLUSTERED 
(
	[MemberName] ASC,
	[WebsiteUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebsiteHeading]    Script Date: 8/3/2021 7:23:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebsiteHeading](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[HeadingText] [nvarchar](120) NOT NULL,
 CONSTRAINT [PK_WebsiteHeading] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_WebsiteHeading_HeadingText]    Script Date: 8/3/2021 7:23:36 PM ******/
CREATE NONCLUSTERED INDEX [IX_WebsiteHeading_HeadingText] ON [dbo].[WebsiteHeading]
(
	[MemberId] ASC
)
INCLUDE([HeadingText]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Friendship] ADD  CONSTRAINT [DF_Friendship_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[Friendship]  WITH CHECK ADD  CONSTRAINT [FK_Friendship_Member1] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Friendship] CHECK CONSTRAINT [FK_Friendship_Member1]
GO
ALTER TABLE [dbo].[Friendship]  WITH CHECK ADD  CONSTRAINT [FK_Friendship_Member2] FOREIGN KEY([FriendMemberId])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Friendship] CHECK CONSTRAINT [FK_Friendship_Member2]
GO
ALTER TABLE [dbo].[WebsiteHeading]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteHeading_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebsiteHeading] CHECK CONSTRAINT [FK_WebsiteHeading_Member]
GO
/****** Object:  StoredProcedure [dbo].[Friendship_MutualSearch]    Script Date: 8/3/2021 7:23:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Friendship_MutualSearch] 
	@MemberId		int = NULL, 
	@SearchPhrase	nvarchar(50) = NULL
AS
BEGIN
	-- This searches for members that are not already friends with the given member,
	-- but have a mutual friend with the member, and that have the given search phrase
	-- in 1 of their website headings.

	SET NOCOUNT ON;

	;With cte_MemberFriends As (
		Select FriendMemberId
		From Friendship
		Where MemberId = @MemberId
	)
	Select
		mutualMember.MemberName As MutualFriend,
		potentialFriend.Id As PotentialFriendId,
		potentialFriend.MemberName As PotentialFriend,
		headings.HeadingText
	From
		cte_MemberFriends As myFriends
	Inner Join
		Friendship As theirFriends On myFriends.FriendMemberId = theirFriends.MemberId
	Inner Join
		Member As mutualMember On myFriends.FriendMemberId = mutualMember.Id 
	Inner Join
		Member As potentialFriend On theirFriends.FriendMemberId = potentialFriend.Id
	Cross Apply (
		Select Top 1 HeadingText
		From WebsiteHeading
		Where
			MemberId = theirFriends.FriendMemberId
			And HeadingText Like '%' + @SearchPhrase + '%'
	) headings
	Where
		theirFriends.FriendMemberId <> @MemberId
		And Not Exists (
			Select 1
			From cte_MemberFriends
			Where FriendMemberId = theirFriends.FriendMemberId
		)
END
GO
