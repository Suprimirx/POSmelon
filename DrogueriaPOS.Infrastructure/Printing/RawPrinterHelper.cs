using System.Runtime.InteropServices;

namespace DrogueriaPOS.Infrastructure.Printing;
internal static class RawPrinterHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)] public string? pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
    }

    [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA pDocInfo);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    public static void SendRawBytes(string printerName, byte[] bytes)
    {
        if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
        {
            var errorCode = Marshal.GetLastWin32Error();
            throw new InvalidOperationException(
                $"No se pudo abrir la impresora '{printerName}'. " +
                $"Código Win32: {errorCode}.");
        }

        var docInfo = new DOCINFOA
        {
            pDocName = "ESC/POS Receipt",
            pOutputFile = null,
            pDataType = "RAW"
        };

        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            StartDocPrinter(hPrinter, 1, ref docInfo);
            StartPagePrinter(hPrinter);
            WritePrinter(hPrinter, handle.AddrOfPinnedObject(), bytes.Length, out _);
            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
        }
        finally
        {
            handle.Free();
            ClosePrinter(hPrinter);
        }
    }
}

