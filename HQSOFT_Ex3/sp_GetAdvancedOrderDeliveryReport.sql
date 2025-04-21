CREATE PROCEDURE sp_GetAdvancedOrderDeliveryReport
    @StartDate DATE,  -- Bắt buộc
    @EndDate DATE,    -- Bắt buộc
    @Country VARCHAR(100) = NULL, 
    @City VARCHAR(100) = NULL,
    @VIPOnly BIT = NULL,
    @Category VARCHAR(100) = NULL,
    @RegionName VARCHAR(100) = NULL,
    @StaffID INT = NULL,
    @MinOrderAmount DECIMAL(18,2) = NULL,
    @MaxOrderAmount DECIMAL(18,2) = NULL,
    @OrderStatus VARCHAR(50) = NULL,
    @DeliveryStatus VARCHAR(50) = NULL,
    @IncludeDiscounts BIT = NULL
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX) = ''
    DECLARE @params NVARCHAR(MAX) = N'
        @StartDate DATE, @EndDate DATE, @Country VARCHAR(100), @City VARCHAR(100),
        @VIPOnly BIT, @Category VARCHAR(100), @RegionName VARCHAR(100), @StaffID INT,
        @MinOrderAmount DECIMAL(18,2), @MaxOrderAmount DECIMAL(18,2),
        @OrderStatus VARCHAR(50), @DeliveryStatus VARCHAR(50), @IncludeDiscounts BIT'

    SET @sql = N'
        SELECT 
            o.OrderID,
            o.OrderDate,
            c.CustomerName,
            c.Country,
            c.City,
            c.VIPStatus,
            SUM(CASE WHEN ISNULL(@IncludeDiscounts, 0) = 1 
				THEN oi.Quantity * p.Price * (1 - ISNULL(p.DiscountPercentage, 0) / 100)
				ELSE oi.Quantity * p.Price 
			END) AS TotalOrderAmount,
            SUM(oi.Quantity) AS TotalQuantity,
            s.ShipmentDate,
            s.DeliveryStatus,
            o.Status AS OrderStatus,
            ds.StaffName AS DeliveryStaffName,
            r.RegionName
        FROM Orders o
        JOIN Customers c ON o.CustomerID = c.CustomerID
        JOIN OrderItems oi ON o.OrderID = oi.OrderID
        JOIN Products p ON oi.ProductID = p.ProductID
        LEFT JOIN Shipments s ON o.OrderID = s.OrderID
        LEFT JOIN DeliveryStaff ds ON s.AssignedStaffID = ds.StaffID
        LEFT JOIN Regions r ON s.RegionID = r.RegionID
        WHERE o.OrderDate BETWEEN @StartDate AND @EndDate'
    
    IF @VIPOnly = 1
        SET @sql += N' AND c.VIPStatus = 1'
    IF @Country IS NOT NULL 
        SET @sql += N' AND c.Country = @Country'
    IF @City IS NOT NULL
        SET @sql += N' AND c.City = @City'
    IF @Category IS NOT NULL
        SET @sql += N' AND p.Category = @Category'
    IF @RegionName IS NOT NULL
        SET @sql += N' AND r.RegionName = @RegionName'
    IF @StaffID IS NOT NULL
        SET @sql += N' AND ds.StaffID = @StaffID'
    IF @OrderStatus IS NOT NULL
        SET @sql += N' AND o.Status = @OrderStatus'
    IF @DeliveryStatus IS NOT NULL
        SET @sql += N' AND s.DeliveryStatus = @DeliveryStatus'

    SET @sql += N'
        GROUP BY o.OrderID, o.OrderDate, c.CustomerName, c.Country, c.City, c.VIPStatus,
                 s.ShipmentDate, s.DeliveryStatus, o.Status, ds.StaffName, r.RegionName'

    IF @MinOrderAmount IS NOT NULL OR @MaxOrderAmount IS NOT NULL
	BEGIN
		SET @sql += N' HAVING 1=1'
		IF @MinOrderAmount IS NOT NULL
			SET @sql += N' AND SUM(CASE 
				WHEN ISNULL(@IncludeDiscounts, 0) = 1 THEN oi.Quantity * p.Price * (1 - ISNULL(p.DiscountPercentage, 0) / 100)
				ELSE oi.Quantity * p.Price 
			END) >= @MinOrderAmount'
		IF @MaxOrderAmount IS NOT NULL
			SET @sql += N' AND SUM(CASE 
				WHEN ISNULL(@IncludeDiscounts, 0) = 1 THEN oi.Quantity * p.Price * (1 - ISNULL(p.DiscountPercentage, 0) / 100)
				ELSE oi.Quantity * p.Price 
			END) <= @MaxOrderAmount'
	END

    SET @sql += N' ORDER BY o.OrderDate'

    EXEC sp_executesql @sql, @params,
        @StartDate=@StartDate, @EndDate=@EndDate, @Country=@Country,
        @City=@City, @VIPOnly=@VIPOnly, @Category=@Category, @RegionName=@RegionName,
        @StaffID=@StaffID, @MinOrderAmount=@MinOrderAmount, @MaxOrderAmount=@MaxOrderAmount,
        @OrderStatus=@OrderStatus, @DeliveryStatus=@DeliveryStatus, @IncludeDiscounts=@IncludeDiscounts

END
