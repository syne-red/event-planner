USE [EventPlanner];

CREATE TABLE [User] (
	 [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	 [Email] VARCHAR(255) NOT NULL,
	 [PasswordHash] VARCHAR(255) NOT NULL
);

CREATE TABLE [Event] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[CreatorId] INT NOT NULL FOREIGN KEY REFERENCES [User](Id),
	[Name] VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	[MaxParticipant] INT NOT NULL,
	[Date] DATETIME NOT NULL,
	[Location] VARCHAR(255) NOT NULL
);

CREATE TABLE [Role] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Role] varchar(64) NOT NULL
);

CREATE TABLE [UserRole] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [User](Id),
	[RoleId] INT NOT NULL FOREIGN KEY REFERENCES [Role](Id)
);

CREATE TABLE [Participant] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [User](Id),
	[EventId] INT NOT NULL FOREIGN KEY REFERENCES [Event](Id)
);

CREATE TABLE [ChatMessage] (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[EventId] INT NOT NULL FOREIGN KEY REFERENCES [Event](Id),
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [User](Id),
	[Date] DATETIME NOT NULL,
	[Message] TEXT NOT NULL
);