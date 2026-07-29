using HvacERP.Domain.WorkOrders;

namespace HvacERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderSignatureTests
{
    private static readonly Guid SignatureId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid WorkOrderId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid SignatureDocumentId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid CapturedByUserId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid VoidedByUserId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

    private static readonly DateTimeOffset SignedAtUtc =
        new(
            2026,
            7,
            28,
            16,
            30,
            0,
            TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesSignature()
    {
        WorkOrderSignature signature =
            CreateSignature();

        Assert.Equal(
            SignatureId,
            signature.Id);

        Assert.Equal(
            WorkOrderId,
            signature.WorkOrderId);

        Assert.Equal(
            SignatureDocumentId,
            signature.SignatureDocumentId);

        Assert.Equal(
            WorkOrderSignatureType.WorkCompletion,
            signature.SignatureType);

        Assert.Equal(
            WorkOrderSignatureStatus.Signed,
            signature.Status);

        Assert.Equal(
            "Jane Customer",
            signature.SignerName);

        Assert.Equal(
            "jane@example.com",
            signature.SignerEmail);

        Assert.Equal(
            "Property owner",
            signature.SignerRelationship);

        Assert.Equal(
            "I confirm that the described work was completed.",
            signature.Attestation);

        Assert.Equal(
            SignedAtUtc,
            signature.SignedAtUtc);

        Assert.Equal(
            CapturedByUserId,
            signature.CapturedByUserId);

        Assert.Equal(
            "192.0.2.10",
            signature.IpAddress);

        Assert.Equal(
            "Technician mobile application",
            signature.DeviceInformation);

        Assert.False(
            signature.IsVoided);
    }

    [Fact]
    public void Create_NormalizesTextValues()
    {
        WorkOrderSignature signature =
            WorkOrderSignature.Create(
                SignatureId,
                WorkOrderId,
                SignatureDocumentId,
                WorkOrderSignatureType.CustomerApproval,
                "  Jane Customer  ",
                "  jane@example.com  ",
                "  Property owner  ",
                "  I authorize the proposed work.  ",
                SignedAtUtc,
                CapturedByUserId,
                "  192.0.2.10  ",
                "  Tablet  ");

        Assert.Equal(
            "Jane Customer",
            signature.SignerName);

        Assert.Equal(
            "jane@example.com",
            signature.SignerEmail);

        Assert.Equal(
            "Property owner",
            signature.SignerRelationship);

        Assert.Equal(
            "I authorize the proposed work.",
            signature.Attestation);

        Assert.Equal(
            "192.0.2.10",
            signature.IpAddress);

        Assert.Equal(
            "Tablet",
            signature.DeviceInformation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankOptionalValues_StoresNull(
        string? value)
    {
        WorkOrderSignature signature =
            WorkOrderSignature.Create(
                SignatureId,
                WorkOrderId,
                SignatureDocumentId,
                WorkOrderSignatureType.WorkAuthorization,
                "Jane Customer",
                value,
                value,
                "I authorize the work.",
                SignedAtUtc,
                CapturedByUserId,
                value,
                value);

        Assert.Null(
            signature.SignerEmail);

        Assert.Null(
            signature.SignerRelationship);

        Assert.Null(
            signature.IpAddress);

        Assert.Null(
            signature.DeviceInformation);
    }

    [Fact]
    public void Create_WithEmptySignatureId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    Guid.Empty,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyWorkOrderId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    Guid.Empty,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "workOrderId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyDocumentId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    Guid.Empty,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "signatureDocumentId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCapturedByUserId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    Guid.Empty,
                    null,
                    null));

        Assert.Equal(
            "capturedByUserId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidSignatureType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkOrderSignature.Create(
                SignatureId,
                WorkOrderId,
                SignatureDocumentId,
                (WorkOrderSignatureType)500,
                "Jane Customer",
                null,
                null,
                "Work completed.",
                SignedAtUtc,
                CapturedByUserId,
                null,
                null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankSignerName_ThrowsArgumentException(
        string signerName)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    signerName,
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "signerName",
            exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankAttestation_ThrowsArgumentException(
        string attestation)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    null,
                    null,
                    attestation,
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "attestation",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithSignerNameLargerThanMaximum_ThrowsArgumentException()
    {
        string signerName =
            new(
                'A',
                WorkOrderSignature.MaximumSignerNameLength + 1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    signerName,
                    null,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "signerName",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmailLargerThanMaximum_ThrowsArgumentException()
    {
        string email =
            new(
                'A',
                WorkOrderSignature.MaximumSignerEmailLength + 1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderSignature.Create(
                    SignatureId,
                    WorkOrderId,
                    SignatureDocumentId,
                    WorkOrderSignatureType.WorkCompletion,
                    "Jane Customer",
                    email,
                    null,
                    "Work completed.",
                    SignedAtUtc,
                    CapturedByUserId,
                    null,
                    null));

        Assert.Equal(
            "signerEmail",
            exception.ParamName);
    }

    [Fact]
    public void Void_WithValidValues_VoidsSignature()
    {
        WorkOrderSignature signature =
            CreateSignature();

        DateTimeOffset voidedAtUtc =
            SignedAtUtc.AddMinutes(10);

        signature.Void(
            "  Customer signed the wrong work order.  ",
            voidedAtUtc,
            VoidedByUserId);

        Assert.True(
            signature.IsVoided);

        Assert.Equal(
            WorkOrderSignatureStatus.Voided,
            signature.Status);

        Assert.Equal(
            "Customer signed the wrong work order.",
            signature.VoidReason);

        Assert.Equal(
            voidedAtUtc,
            signature.VoidedAtUtc);

        Assert.Equal(
            VoidedByUserId,
            signature.VoidedByUserId);

        Assert.Equal(
            voidedAtUtc,
            signature.LastModifiedAtUtc);

        Assert.Equal(
            VoidedByUserId,
            signature.LastModifiedByUserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Void_WithBlankReason_ThrowsArgumentException(
        string reason)
    {
        WorkOrderSignature signature =
            CreateSignature();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => signature.Void(
                    reason,
                    SignedAtUtc.AddMinutes(10),
                    VoidedByUserId));

        Assert.Equal(
            "reason",
            exception.ParamName);
    }

    [Fact]
    public void Void_WithReasonLargerThanMaximum_ThrowsArgumentException()
    {
        WorkOrderSignature signature =
            CreateSignature();

        string reason =
            new(
                'A',
                WorkOrderSignature.MaximumVoidReasonLength + 1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => signature.Void(
                    reason,
                    SignedAtUtc.AddMinutes(10),
                    VoidedByUserId));

        Assert.Equal(
            "reason",
            exception.ParamName);
    }

    [Fact]
    public void Void_WithEmptyUserId_ThrowsArgumentException()
    {
        WorkOrderSignature signature =
            CreateSignature();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => signature.Void(
                    "Incorrect signature.",
                    SignedAtUtc.AddMinutes(10),
                    Guid.Empty));

        Assert.Equal(
            "voidedByUserId",
            exception.ParamName);
    }

    [Fact]
    public void Void_BeforeSignedTime_ThrowsArgumentException()
    {
        WorkOrderSignature signature =
            CreateSignature();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => signature.Void(
                    "Incorrect signature.",
                    SignedAtUtc.AddMinutes(-1),
                    VoidedByUserId));

        Assert.Equal(
            "voidedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void Void_WhenAlreadyVoided_ThrowsInvalidOperationException()
    {
        WorkOrderSignature signature =
            CreateSignature();

        signature.Void(
            "Incorrect signature.",
            SignedAtUtc.AddMinutes(10),
            VoidedByUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => signature.Void(
                    "Void again.",
                    SignedAtUtc.AddMinutes(20),
                    VoidedByUserId));

        Assert.Equal(
            "The work order signature has already been voided.",
            exception.Message);
    }

    [Fact]
    public void SignedDetails_RemainAvailableAfterVoid()
    {
        WorkOrderSignature signature =
            CreateSignature();

        signature.Void(
            "Incorrect work order.",
            SignedAtUtc.AddMinutes(10),
            VoidedByUserId);

        Assert.Equal(
            "Jane Customer",
            signature.SignerName);

        Assert.Equal(
            SignatureDocumentId,
            signature.SignatureDocumentId);

        Assert.Equal(
            SignedAtUtc,
            signature.SignedAtUtc);

        Assert.Equal(
            "I confirm that the described work was completed.",
            signature.Attestation);
    }

    private static WorkOrderSignature CreateSignature()
    {
        return WorkOrderSignature.Create(
            SignatureId,
            WorkOrderId,
            SignatureDocumentId,
            WorkOrderSignatureType.WorkCompletion,
            "Jane Customer",
            "jane@example.com",
            "Property owner",
            "I confirm that the described work was completed.",
            SignedAtUtc,
            CapturedByUserId,
            "192.0.2.10",
            "Technician mobile application");
    }
}
