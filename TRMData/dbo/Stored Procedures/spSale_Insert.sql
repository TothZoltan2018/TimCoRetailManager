CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
	@SaleDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
begin
	set nocount on;

	insert into [dbo].Sale(CashierId, SaleDate, SubTotal, Tax, Total)
	values(@CashierId, @SaleDate, @SubTotal, @Tax, @Total);

	--select @Id = @@IDENTITY; -- Returns the lastly created identity created by anything
	select @Id = Scope_IDENTITY(); --Returns the lastly created identity within this procedure
end

