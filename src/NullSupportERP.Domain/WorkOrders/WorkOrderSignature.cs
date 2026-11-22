using NullSupportERP.Domain.Common.Base;

namespace NullSupportERP.Domain.WorkOrders;

public sealed class WorkOrderSignature : AggregateRoot
{
    public const int MaximumSignerNameLength = 200;

    public const int MaximumSignerEmailLength = 320;

    public const int MaximumSignerRelationshipLength = 100;

    public const int MaximumAttestationLength = 2_000;

    public const int MaximumIpAddressLength = 64;

    public const int MaximumDeviceInformationLength = 500;

    public const int MaximumVoidReasonLength = 500;

    public Guid WorkOrderId { get; private set; }

    public Guid SignatureDocumentId { get; private set; }

    public WorkOrderSignatureType SignatureType { get; private set; }

    public WorkOrderSignatureStatus Status { get; private set; }

    public string SignerName { get; private set; }

    public string? SignerEmail { get; private set; }

    public string? SignerRelationship { get; private set; }

    public string Attestation { get; private set; }

    public DateTimeOffset SignedAtUtc { get; private set; }

    public Guid CapturedByUserId { get; private set; }

    public string? IpAddress { get; private set; }

    public string? DeviceInformation { get; private set; }

    public DateTimeOffset? VoidedAtUtc { get; private set; }

    public Guid? VoidedByUserId { get; private set; }

    public string? VoidReason { get; private set; }

    public bool IsVoided =>
        Status == WorkOrderSignatureStatus.Voided;

    private WorkOrderSignature(
        Guid id,
        Guid workOrderId,
        Guid signatureDocumentId,
        WorkOrderSignatureType signatureType,
        string signerName,
        string? signerEmail,
        string? signerRelationship,
        string attestation,
        DateTimeOffset signedAtUtc,
        Guid capturedByUserId,
        string? ipAddress,
        string? deviceInformation)
        : base(
            id,
            signedAtUtc,
            capturedByUserId)
    {
        WorkOrderId = workOrderId;
        SignatureDocumentId = signatureDocumentId;
        SignatureType = signatureType;
        Status = WorkOrderSignatureStatus.Signed;
        SignerName = signerName;
        SignerEmail = signerEmail;
        SignerRelationship = signerRelationship;
        Attestation = attestation;
        SignedAtUtc = signedAtUtc;
        CapturedByUserId = capturedByUserId;
        IpAddress = ipAddress;
        DeviceInformation = deviceInformation;
    }

    public static WorkOrderSignature Create(
        Guid id,
        Guid workOrderId,
        Guid signatureDocumentId,
        WorkOrderSignatureType signatureType,
        string signerName,
        string? signerEmail,
        string? signerRelationship,
        string attestation,
        DateTimeOffset signedAtUtc,
        Guid capturedByUserId,
        string? ipAddress,
        string? deviceInformation)
    {
        ValidateId(
            id,
            nameof(id),
            "Work order signature ID");

        ValidateId(
            workOrderId,
            nameof(workOrderId),
            "Work order ID");

        ValidateId(
            signatureDocumentId,
            nameof(signatureDocumentId),
            "Signature document ID");

        ValidateSignatureType(signatureType);

        string normalizedSignerName =
            NormalizeRequired(
                signerName,
                MaximumSignerNameLength,
                nameof(signerName),
                "Signer name");

        string? normalizedSignerEmail =
            NormalizeOptional(
                signerEmail,
                MaximumSignerEmailLength,
                nameof(signerEmail),
                "Signer email");

        string? normalizedSignerRelationship =
            NormalizeOptional(
                signerRelationship,
                MaximumSignerRelationshipLength,
                nameof(signerRelationship),
                "Signer relationship");

        string normalizedAttestation =
            NormalizeRequired(
                attestation,
                MaximumAttestationLength,
                nameof(attestation),
                "Attestation");

        ValidateId(
            capturedByUserId,
            nameof(capturedByUserId),
            "Captured by user ID");

        string? normalizedIpAddress =
            NormalizeOptional(
                ipAddress,
                MaximumIpAddressLength,
                nameof(ipAddress),
                "IP address");

        string? normalizedDeviceInformation =
            NormalizeOptional(
                deviceInformation,
                MaximumDeviceInformationLength,
                nameof(deviceInformation),
                "Device information");

        return new WorkOrderSignature(
            id,
            workOrderId,
            signatureDocumentId,
            signatureType,
            normalizedSignerName,
            normalizedSignerEmail,
            normalizedSignerRelationship,
            normalizedAttestation,
            signedAtUtc,
            capturedByUserId,
            normalizedIpAddress,
            normalizedDeviceInformation);
    }

    public void Void(
        string reason,
        DateTimeOffset voidedAtUtc,
        Guid voidedByUserId)
    {
        if (IsVoided)
        {
            throw new InvalidOperationException(
                "The work order signature has already been voided.");
        }

        string normalizedReason =
            NormalizeRequired(
                reason,
                MaximumVoidReasonLength,
                nameof(reason),
                "Void reason");

        ValidateId(
            voidedByUserId,
            nameof(voidedByUserId),
            "Voided by user ID");

        if (voidedAtUtc < SignedAtUtc)
        {
            throw new ArgumentException(
                "A signature cannot be voided before it was signed.",
                nameof(voidedAtUtc));
        }

        Status = WorkOrderSignatureStatus.Voided;
        VoidReason = normalizedReason;
        VoidedAtUtc = voidedAtUtc;
        VoidedByUserId = voidedByUserId;

        MarkModified(
            voidedAtUtc,
            voidedByUserId);
    }

    private static void ValidateSignatureType(
        WorkOrderSignatureType signatureType)
    {
        if (!Enum.IsDefined(signatureType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(signatureType),
                signatureType,
                "Work order signature type is invalid.");
        }
    }

    private static void ValidateId(
        Guid value,
        string parameterName,
        string displayName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }
    }

    private static string NormalizeRequired(
        string value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }

    private static string? NormalizeOptional(
        string? value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }
}
