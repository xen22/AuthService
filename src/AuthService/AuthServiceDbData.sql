USE AuthServiceDb;

INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, PasswordHash, IsAdmin, AccessFailedCount, EmailConfirmed, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled)
  VALUES (1, "user1", "user1", "user1", 0, 100, 0, 0, 0, 0),
         (2, "user2", "user2", "user2", 0, 100, 0, 0, 0, 0),
         (3, "demo", "demo", "demo", 0, 100, 0, 0, 0, 0),
         (4, "admin", "admin", "admin", 1, 100, 0, 0, 0, 0);