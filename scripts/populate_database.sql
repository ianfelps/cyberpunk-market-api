-- Script de seed para SQL Server – Cyberpunk Market

SET NOCOUNT ON;

-- ========================
-- IDs
-- ========================
DECLARE @Buyer1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Buyer2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @SellerUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @SellerId UNIQUEIDENTIFIER = NEWID();

DECLARE @CategoryWeaponsId UNIQUEIDENTIFIER = NEWID();
DECLARE @CategoryCyberwareId UNIQUEIDENTIFIER = NEWID();
DECLARE @CategoryDrugsId UNIQUEIDENTIFIER = NEWID();

DECLARE @ProductPistolId UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductKatanaId UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductCyberEyeId UNIQUEIDENTIFIER = NEWID();
DECLARE @ProductStimId UNIQUEIDENTIFIER = NEWID();

DECLARE @Buyer1CartId UNIQUEIDENTIFIER = NEWID();

DECLARE @WishlistItem1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @WishlistItem2Id UNIQUEIDENTIFIER = NEWID();

DECLARE @Buyer1AddressId UNIQUEIDENTIFIER = NEWID();

DECLARE @Order1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @OrderItem1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @OrderItem2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Payment1Id UNIQUEIDENTIFIER = NEWID();

DECLARE @Review1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Review2Id UNIQUEIDENTIFIER = NEWID();

-- ========================
-- Users (Buyer, Seller)
-- PasswordHash: exemplo para "123"
-- ========================
INSERT INTO Users (Id, Name, Email, PasswordHash, Role, CreatedAt, UpdatedAt)
VALUES
(@Buyer1Id, 'V Merc', 'v.merc@example.com', '$2a$11$6OGyA7cVTUx2Orr9rLPBper5i.8D/0CmGpRWvd0nJ9IJ.cdiHuQVq', 1, GETUTCDATE(), NULL),
(@Buyer2Id, 'Lucy Net', 'lucy.net@example.com', '$2a$11$6OGyA7cVTUx2Orr9rLPBper5i.8D/0CmGpRWvd0nJ9IJ.cdiHuQVq', 1, GETUTCDATE(), NULL),
(@SellerUserId, 'Dex Fixer', 'dex.fixer@example.com', '$2a$11$6OGyA7cVTUx2Orr9rLPBper5i.8D/0CmGpRWvd0nJ9IJ.cdiHuQVq', 2, GETUTCDATE(), NULL);

-- ========================
-- Seller
-- ========================
INSERT INTO Sellers (Id, UserId, StoreName, Bio, CreatedAt, UpdatedAt)
VALUES
(@SellerId, @SellerUserId, 'Afterlife Arms', 'High-end illegal hardware for the right eddies.', GETUTCDATE(), NULL);

-- ========================
-- Categories
-- ========================
INSERT INTO Categories (Id, Name, Slug, CreatedAt, UpdatedAt)
VALUES
(@CategoryWeaponsId, 'Weapons', 'weapons', GETUTCDATE(), NULL),
(@CategoryCyberwareId, 'Cyberware', 'cyberware', GETUTCDATE(), NULL),
(@CategoryDrugsId, 'Chems & Stims', 'chems-stims', GETUTCDATE(), NULL);

-- ========================
-- Products
-- ========================
INSERT INTO Products (Id, Name, Description, Price, StockQuantity, IsActive, SellerId, CategoryId, CreatedAt, UpdatedAt)
VALUES
(@ProductPistolId, 'Militech M-10AF Lexington', 'Compact smart pistol, corpo-grade, low recoil.', 1200.00, 50, 1, @SellerId, @CategoryWeaponsId, GETUTCDATE(), NULL),
(@ProductKatanaId, 'Kuroshi Mono-Katana', 'Ultra-sharp monoblade with heated edge.', 3500.00, 15, 1, @SellerId, @CategoryWeaponsId, GETUTCDATE(), NULL),
(@ProductCyberEyeId, 'Kiroshi Optics Mk.3', 'Cybernetic eye implant with enhanced zoom and targeting.', 2800.00, 20, 1, @SellerId, @CategoryCyberwareId, GETUTCDATE(), NULL),
(@ProductStimId, 'NovaStim XR', 'Combat stimpack for short-term reflex boost.', 450.00, 100, 1, @SellerId, @CategoryDrugsId, GETUTCDATE(), NULL);

-- ========================
-- Cart (Buyer1)
-- ========================
INSERT INTO Carts (Id, UserId, CreatedAt, UpdatedAt)
VALUES
(@Buyer1CartId, @Buyer1Id, GETUTCDATE(), NULL);

-- ========================
-- CartItems (Buyer1)
-- ========================
INSERT INTO CartItems (Id, CartId, ProductId, Quantity, CreatedAt, UpdatedAt)
VALUES
(NEWID(), @Buyer1CartId, @ProductPistolId, 1, GETUTCDATE(), NULL),
(NEWID(), @Buyer1CartId, @ProductStimId, 3, GETUTCDATE(), NULL);

-- ========================
-- Wishlist (Buyer1)
-- ========================
INSERT INTO WishlistItems (Id, UserId, ProductId, NotifyOnPriceDrop, CreatedAt, UpdatedAt)
VALUES
(@WishlistItem1Id, @Buyer1Id, @ProductCyberEyeId, 1, GETUTCDATE(), NULL),
(@WishlistItem2Id, @Buyer1Id, @ProductKatanaId, 1, GETUTCDATE(), NULL);

-- ========================
-- Address (Buyer1)
-- ========================
INSERT INTO Addresses (Id, UserId, Street, Number, Complement, Neighborhood, City, State, ZipCode, IsDefault, CreatedAt, UpdatedAt)
VALUES
(@Buyer1AddressId, @Buyer1Id, 'Night City Ave', '101', 'Apt 13', 'Watson', 'Night City', 'NC', '2077-001', 1, GETUTCDATE(), NULL);

-- ========================
-- Order + Items + Payment (Buyer1)
-- ========================
INSERT INTO Orders (Id, BuyerId, ShippingAddressId, OrderDate, TotalAmount, Status, CreatedAt, UpdatedAt)
VALUES
(@Order1Id, @Buyer1Id, @Buyer1AddressId, DATEADD(DAY, -1, GETUTCDATE()), 1200.00 + (2 * 450.00), 2, GETUTCDATE(), NULL); -- Paid

INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, UnitPrice, Discount, CreatedAt, UpdatedAt)
VALUES
(@OrderItem1Id, @Order1Id, @ProductPistolId, 1, 1200.00, 0.00, GETUTCDATE(), NULL),
(@OrderItem2Id, @Order1Id, @ProductStimId, 2, 450.00, 0.00, GETUTCDATE(), NULL);

INSERT INTO Payments (Id, OrderId, Amount, Method, Status, ExternalId, PaidAt, CreatedAt, UpdatedAt)
VALUES
(@Payment1Id, @Order1Id, 2100.00, 3, 2, 'PIX-TXN-777777', DATEADD(HOUR, -12, GETUTCDATE()), GETUTCDATE(), NULL);

-- ========================
-- Reviews
-- ========================
INSERT INTO Reviews (Id, UserId, ProductId, Rating, Comment, CreatedAt, UpdatedAt)
VALUES
(@Review1Id, @Buyer1Id, @ProductPistolId, 5, 'Clean trigger, no jams. Edgerunners material.', GETUTCDATE(), NULL),
(@Review2Id, @Buyer2Id, @ProductCyberEyeId, 4, 'Great optics, slight lag under heavy load.', GETUTCDATE(), NULL);

SET NOCOUNT OFF;