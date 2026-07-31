//using DrogueriaPOS.Application.Repositories;
//using DrogueriaPOS.Application.Services;
//using DrogueriaPOS.Domain.Entities;
//using FluentAssertions;
//using Moq;

//namespace DrogueriaPOS.Tests.Services;
//public class InventoryServiceTests
//{
//    private readonly Mock<IProductRepository> _productRepoMock;

//    private readonly InventoryService _sut;

//    public InventoryServiceTests()
//    {
//        _productRepoMock = new Mock<IProductRepository>();
//        _sut = new InventoryService(_productRepoMock.Object);
//    }

//    private static Product CreateSampleProduct(
//        string barcode = "7702001001",
//        string brandName = "Acetaminofén",
//        decimal salePrice = 1500m,
//        decimal ivaPercentage = 0m,
//        int initialStock = 10)
//    {
//        return new Product(
//            barcode: barcode,
//            brandName: brandName,
//            salePrice: salePrice,
//            IvaPercentage: ivaPercentage,
//            initialStock: initialStock,
//            genericName: "Paracetamol",
//            concentration: "500mg",
//            presentation: "Tabletas x 10",
//            invimaRegistration: "INVIMA-TEST-123"
//        );
//    }

//    [Fact]
//    public async Task GetProductByIdAsync_ProductExists_ReturnsSuccess()
//    {
//        // Arrange
//        var product = CreateSampleProduct();
//        _productRepoMock.Setup(repo => repo.GetByIdAsync(1))
//            .ReturnsAsync(product);
//        // Act
//        var result = await _sut.GetProductByIdAsync(1);
//        // Assert
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().Be(product);
//    }

//    [Fact]
//    public async Task GetProductByIdAsync_ProductNotExists_ReturnFailure()
//    {
//        // ARRANGE: el repo devuelve null → producto no encontrado
//        _productRepoMock
//            .Setup(r => r.GetByIdAsync(99))
//            .ReturnsAsync((Product?)null);

//        // ACT
//        var result = await _sut.GetProductByIdAsync(99);

//        // ASSERT
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("Producto no encontrado.");
//    }

//    [Fact]
//    public async Task GetAllProductsAsync_AlwaysReturnSuccess()
//    {
//        // ARRANGE
//        var products = new List<Product>
//        {
//            CreateSampleProduct("111", "Ibuprofeno"),
//            CreateSampleProduct("222", "Amoxicilina")
//        };
//        _productRepoMock
//            .Setup(r => r.GetAllAsync())
//            .ReturnsAsync(products);

//        // ACT
//        var result = await _sut.GetAllProductsAsync();

//        // ASSERT: la operación nunca falla; si el repo devuelve lista
//        // vacía, igual es Success con colección vacía.
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().HaveCount(2);
//    }

//    [Fact]
//    public async Task GetAllProductsAsync_WithoutProducts_ReturnSuccessWithListEmpty()
//    {
//        // ARRANGE
//        _productRepoMock
//            .Setup(r => r.GetAllAsync())
//            .ReturnsAsync(new List<Product>());

//        // ACT
//        var result = await _sut.GetAllProductsAsync();

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().BeEmpty();
//    }

//    [Fact]
//    public async Task GetActivesProductsAsync_ReturnProductsActives()
//    {
//        // ARRANGE: el repo ya filtra los activos; el servicio solo
//        // lo delega. Nos aseguramos de que pasa el resultado tal cual.
//        var activeProducts = new List<Product> { CreateSampleProduct() };
//        _productRepoMock
//            .Setup(r => r.GetActivesAsync())
//            .ReturnsAsync(activeProducts);

//        // ACT
//        var result = await _sut.GetActivesProductsAsync();

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().HaveCount(1);
//    }

//    [Fact]
//    public async Task CreateProductAsync_BarcodeExists_ReturnFailure()
//    {
//        // ARRANGE: el barcode ya está registrado en BD
//        _productRepoMock
//            .Setup(r => r.BarCodeExistsAsync("7702001001"))
//            .ReturnsAsync(true);

//        // ACT
//        var result = await _sut.CreateProductAsync(
//            barCode: "7702001001",
//            brandName: "Acetaminofén",
//            genericName: "Paracetamol",
//            concentration: "500mg",
//            presentation: "Tabletas",
//            invimaRegistration: "INVIMA123",
//            salePrice: 1500m,
//            ivaPercentage: 0m,
//            initialStock: 10
//        );

//        // ASSERT: falla Y nunca llama a CreateAsync
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("El código de barras ya existe");

//        // Verify comprueba que un método del mock fue (o no fue) llamado.
//        // Times.Never → garantiza que no se persistió nada en BD.
//        _productRepoMock.Verify(
//            r => r.CreateAsync(It.IsAny<Product>()),
//            Times.Never);
//    }

//    [Fact]
//    public async Task CreateProductAsync_ValidData_ReturnSuccessYPersiste()
//    {
//        // ARRANGE
//        _productRepoMock
//            .Setup(r => r.BarCodeExistsAsync("7702001001"))
//            .ReturnsAsync(false);
//        _productRepoMock
//            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
//            .Returns(Task.CompletedTask);

//        // ACT
//        var result = await _sut.CreateProductAsync(
//            barCode: "7702001001",
//            brandName: "Acetaminofén",
//            genericName: "Paracetamol",
//            concentration: "500mg",
//            presentation: "Tabletas",
//            invimaRegistration: "INVIMA123",
//            salePrice: 1500m,
//            ivaPercentage: 0m,
//            initialStock: 10
//        );

//        // ASSERT: el Result trae el producto recién creado
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().NotBeNull();
//        result.Data.BrandName.Should().Be("Acetaminofén");
//        result.Data.Stock.Should().Be(10);
//        result.Data.IsActive.Should().BeTrue();

//        // Times.Once → se llamó exactamente una vez a CreateAsync
//        _productRepoMock.Verify(
//            r => r.CreateAsync(It.IsAny<Product>()),
//            Times.Once);
//    }

//    [Fact]
//    public async Task UpdateProductAsync_ProductNoExists_ReturnFailure()
//    {
//        // ARRANGE
//        _productRepoMock
//            .Setup(r => r.GetByIdAsync(99))
//            .ReturnsAsync((Product?)null);

//        // ACT
//        var result = await _sut.UpdateProductAsync(
//            id: 99, barCode: "111", brandName: "X",
//            genericName: "X", concentration: "X",
//            presentation: "X", invimaRegistration: "X",
//            salePrice: 100m, stock: 5, ivaPercentage: 0m
//        );

//        // ASSERT
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("Producto no encontrado");
//    }

//    [Fact]
//    public async Task UpdateProductAsync_NewBarcodeInUse_ReturnFailure()
//    {
//        // ARRANGE: producto actual tiene barcode "7702001001"
//        //          se intenta cambiar a "9999999999" que ya existe
//        var existingProduct = CreateSampleProduct(barcode: "7702001001");
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
//        _productRepoMock.Setup(r => r.BarCodeExistsAsync("9999999999")).ReturnsAsync(true);

//        // ACT
//        var result = await _sut.UpdateProductAsync(
//            id: 1, barCode: "9999999999", brandName: "Acetaminofén",
//            genericName: "Paracetamol", concentration: "500mg",
//            presentation: "Tabletas", invimaRegistration: "INVIMA123",
//            salePrice: 1500m, stock: 10, ivaPercentage: 0m
//        );

//        // ASSERT
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("El código de barras ya está en uso por otro producto");
//    }

//    [Fact]
//    public async Task UpdateProductAsync_SameBarcode_NoVerifyDuplicate()
//    {
//        // ARRANGE: el barcode no cambia → NO debe llamar a BarCodeExistsAsync
//        var product = CreateSampleProduct(barcode: "7702001001");
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
//        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

//        // ACT: pasamos el mismo barcode que ya tiene el producto
//        var result = await _sut.UpdateProductAsync(
//            id: 1, barCode: "7702001001", brandName: "Acetaminofén",
//            genericName: "Paracetamol", concentration: "500mg",
//            presentation: "Tabletas", invimaRegistration: "INVIMA123",
//            salePrice: 1500m, stock: 10, ivaPercentage: 0m
//        );

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        _productRepoMock.Verify(
//            r => r.BarCodeExistsAsync(It.IsAny<string>()),
//            Times.Never); // No debió verificar duplicados
//    }

//    [Fact]
//    public async Task UpdateProductAsync_StockAumentado_ActualizaStockCorrectamente()
//    {
//        // ARRANGE: stock actual = 10, nuevo stock = 15
//        var product = CreateSampleProduct(initialStock: 10);
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
//        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

//        // ACT
//        var result = await _sut.UpdateProductAsync(
//            id: 1, barCode: "7702001001", brandName: "Acetaminofén",
//            genericName: "Paracetamol", concentration: "500mg",
//            presentation: "Tabletas", invimaRegistration: "INVIMA123",
//            salePrice: 1500m, stock: 15, ivaPercentage: 0m
//        );

//        // ASSERT: el servicio llamó IncreaseStock(5) → stock final = 15
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Stock.Should().Be(15);
//    }

//    [Fact]
//    public async Task UpdateProductAsync_StockReducido_ActualizaStockCorrectamente()
//    {
//        // ARRANGE: stock actual = 10, nuevo stock = 4
//        var product = CreateSampleProduct(initialStock: 10);
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
//        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

//        // ACT
//        var result = await _sut.UpdateProductAsync(
//            id: 1, barCode: "7702001001", brandName: "Acetaminofén",
//            genericName: "Paracetamol", concentration: "500mg",
//            presentation: "Tabletas", invimaRegistration: "INVIMA123",
//            salePrice: 1500m, stock: 4, ivaPercentage: 0m
//        );

//        // ASSERT: el servicio llamó DecreaseStock(6) → stock final = 4
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Stock.Should().Be(4);
//    }

//    [Fact]
//    public async Task UpdateProductAsync_DatosValidos_ActualizaCamposYPersiste()
//    {
//        // ARRANGE
//        var product = CreateSampleProduct();
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
//        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

//        // ACT: cambiamos nombre y precio
//        var result = await _sut.UpdateProductAsync(
//            id: 1, barCode: "7702001001", brandName: "Ibuprofeno",
//            genericName: "Ibuprofeno genérico", concentration: "400mg",
//            presentation: "Cápsulas", invimaRegistration: "INVIMA456",
//            salePrice: 2500m, stock: 10, ivaPercentage: 0m
//        );

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        result.Data.BrandName.Should().Be("Ibuprofeno");
//        result.Data.SalePrice.Should().Be(2500m);
//        _productRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteProductAsync_ProductoNoExiste_RetornaFailure()
//    {
//        // ARRANGE
//        _productRepoMock
//            .Setup(r => r.GetByIdAsync(99))
//            .ReturnsAsync((Product?)null);

//        // ACT
//        var result = await _sut.DeleteProductAsync(99);

//        // ASSERT
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("Producto no encontrado");
//        _productRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
//    }

//    [Fact]
//    public async Task DeleteProductAsync_ProductoExiste_DesactivaYPersiste()
//    {
//        // ARRANGE
//        var product = CreateSampleProduct();
//        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
//        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

//        // ACT
//        var result = await _sut.DeleteProductAsync(1);

//        // ASSERT: el borrado es lógico (IsActive = false), no físico
//        result.IsSuccess.Should().BeTrue();
//        product.IsActive.Should().BeFalse();                         // se desactivó
//        _productRepoMock.Verify(r => r.UpdateAsync(product), Times.Once); // se persistió
//    }

//    [Fact]
//    public async Task GetProductByBarCodeAsync_ProductoEncontrado_RetornaSuccess()
//    {
//        // ARRANGE
//        var product = CreateSampleProduct(barcode: "7702001001");
//        _productRepoMock
//            .Setup(r => r.GetByBarcodeAsync("7702001001"))
//            .ReturnsAsync(product);

//        // ACT
//        var result = await _sut.GetProductByBarCodeAsync("7702001001");

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        result.Data.BarCode.Should().Be("7702001001");
//    }

//    [Fact]
//    public async Task GetProductByBarCodeAsync_ProductoNoEncontrado_RetornaFailure()
//    {
//        // ARRANGE
//        _productRepoMock
//            .Setup(r => r.GetByBarcodeAsync("0000000000"))
//            .ReturnsAsync((Product?)null);

//        // ACT
//        var result = await _sut.GetProductByBarCodeAsync("0000000000");

//        // ASSERT
//        result.IsSuccess.Should().BeFalse();
//        result.ErrorMessage.Should().Be("Producto no encontrado");
//    }

//    [Fact]
//    public async Task GetProductByNameAsync_HayCoincidencias_RetornaListaConResultados()
//    {
//        // ARRANGE
//        var products = new List<Product>
//        {
//            CreateSampleProduct("111", "Acetaminofén 500mg"),
//            CreateSampleProduct("222", "Acetaminofén 1g")
//        };
//        _productRepoMock
//            .Setup(r => r.SearchByNameAsync("Acetaminofén"))
//            .ReturnsAsync(products);

//        // ACT
//        var result = await _sut.GetProductByNameAsync("Acetaminofén");

//        // ASSERT
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().HaveCount(2);
//    }

//    [Fact]
//    public async Task GetProductByNameAsync_SinCoincidencias_RetornaSuccessConListaVacia()
//    {
//        // ARRANGE: búsqueda sin resultados → sigue siendo Success
//        _productRepoMock
//            .Setup(r => r.SearchByNameAsync("ProductoInexistente"))
//            .ReturnsAsync(new List<Product>());

//        // ACT
//        var result = await _sut.GetProductByNameAsync("ProductoInexistente");

//        // ASSERT: el servicio no convierte "sin resultados" en fallo
//        result.IsSuccess.Should().BeTrue();
//        result.Data.Should().BeEmpty();
//    }
//}
