using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TodoSeUsa.Infrastructure.Security;

public static class KestrelCertificateService
{
    public static void EnsureCertificate(string certPath, string password)
    {
        if (File.Exists(certPath))
            return;

        using var rsa = RSA.Create(2048);

        var request = new CertificateRequest(
            "CN=localhost",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                false));

        request.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(5));

        File.WriteAllBytes(
            certPath,
            certificate.Export(X509ContentType.Pfx, password)
        );
    }
}
