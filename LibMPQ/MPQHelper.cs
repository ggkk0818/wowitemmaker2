using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LibMPQ
{
    public class MPQHelper
    {
        /// <summary>
        /// The function creates a MPQ version 1.0 (up to 4 GB). This is the default value
        /// </summary>
        public static uint MPQ_CREATE_ARCHIVE_V1 = 0x00000000;

        /// <summary>
        /// The function creates a MPQ version 2.0 (supports MPQ of size greater than 4 GB).
        /// </summary>
        public static uint MPQ_CREATE_ARCHIVE_V2 = 0x00010000;

        /// <summary>
        /// The function creates a MPQ version 3.0 (introduced in WoW-Cataclysm Beta).
        /// </summary>
        public static uint MPQ_CREATE_ARCHIVE_V3 = 0x00020000;

        /// <summary>
        /// The function creates a MPQ version 4.0 (used in WoW-Cataclysm).
        /// </summary>
        public static uint MPQ_CREATE_ARCHIVE_V4 = 0x00030000;

        /// <summary>
        /// When creating new MPQ, the (attributes) file will be added to it. The (attributes) file contains additional file information, such as file time, CRC32 checksum and MD5 hash.
        /// </summary>
        public static uint MPQ_CREATE_ATTRIBUTES = 0x00000001;


        public static uint HASH_TABLE_SIZE_MIN = 0x04;
        public static uint HASH_TABLE_SIZE_MAX = 0x80000;

        /// <summary>
        /// The file will be compressed using IMPLODE compression method. This flag cannot be used together with MPQ_FILE_COMPRESS. If this flag is present, then the dwCompression and dwCompressionNext parameters are ignored. This flag is obsolete and was used only in Diablo I.
        /// </summary>
        public static uint MPQ_FILE_IMPLODE = 0x00000100;

        /// <summary>
        /// The file will be compressed. This flag cannot be used together with MPQ_FILE_IMPLODE.
        /// </summary>
        public static uint MPQ_FILE_COMPRESS = 0x00000200;

        /// <summary>
        /// The file will be stored as encrypted.
        /// </summary>
        public static uint MPQ_FILE_ENCRYPTED = 0x00010000;

        /// <summary>
        /// The file's encryption key will be adjusted according to file size in the archive. This flag must be used together with MPQ_FILE_ENCRYPTED.
        /// </summary>
        public static uint MPQ_FILE_FIX_KEY = 0x00020000;

        /// <summary>
        /// The file will have the deletion marker.
        /// </summary>
        public static uint MPQ_FILE_DELETE_MARKER = 0x02000000;

        /// <summary>
        /// The file will have CRC for each file sector. Ignored if the file is not compressed or if the file is stored as single unit.
        /// </summary>
        public static uint MPQ_FILE_SECTOR_CRC = 0x04000000;

        /// <summary>
        /// The file will be added as single unit. Files stored as single unit cannot be encrypted, because Blizzard doesn't support them.
        /// </summary>
        public static uint MPQ_FILE_SINGLE_UNIT = 0x01000000;

        /// <summary>
        /// If this flag is specified and the file is already in the MPQ, it will be replaced.
        /// </summary>
        public static uint MPQ_FILE_REPLACEEXISTING = 0x80000000;

        public static uint MPQ_COMPRESSION_HUFFMANN = 0x01;

        public static uint MPQ_COMPRESSION_ZLIB = 0x02;

        public static uint MPQ_COMPRESSION_PKWARE = 0x08;

        public static uint MPQ_COMPRESSION_BZIP2 = 0x10;

        public static uint MPQ_COMPRESSION_SPARSE = 0x20;

        public static uint MPQ_COMPRESSION_ADPCM_MONO = 0x40;

        public static uint MPQ_COMPRESSION_ADPCM_STEREO = 0x80;

        public static uint MPQ_COMPRESSION_LZMA = 0x12;

        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenArchive(string szMpqName, uint dwPriority, uint dwFlags, out IntPtr phMPQ);

        [DllImport("StormLib.dll")]
        public static extern bool SFileCreateArchive(string szMpqName, uint dwFlags, uint dwMaxFileCount, out IntPtr phMPQ);

        [DllImport("StormLib.dll")]
        public static extern bool SFileAddFileEx(IntPtr hMpq, string szFileName, string szArchivedName, uint dwFlags, uint dwCompression, uint dwCompressionNext);

        [DllImport("StormLib.dll")]
        public static extern bool SFileFlushArchive(IntPtr hMpq);

        [DllImport("StormLib.dll")]
        public static extern bool SFileCloseArchive(IntPtr hMpq);

        [DllImport("StormLib.dll")]
        public static extern int SFileAddListFile(IntPtr hMpq, string szListFile);

        [DllImport("StormLib.dll")]
        public static extern bool SFileCompactArchive(IntPtr hMpq, string szListFile, bool bReserved);

        [DllImport("StormLib.dll")]
        public static extern int SFileSetLocale(int lcNewLocale);

        [DllImport("StormLib.dll")]
        public static extern bool SFileSetMaxFileCount(IntPtr hMpq, uint dwMaxFileCount);
    }
}
