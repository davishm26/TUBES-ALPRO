using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


class Program
{
    // Data pengguna: username, password, dan role (Admin atau User)
    private static Dictionary<string, (string Password, string Role)> users = new Dictionary<string, (string, string)>
    {
        { "admin", ("admin123", "Admin") },
        { "user", ("user123", "User") }
    };

    // Data buku menggunakan List
    private static List<Buku> daftarBuku = new List<Buku>();

    // Data peminjaman untuk masing-masing User
    private static Dictionary<string, List<Buku>> peminjamanBuku = new Dictionary<string, List<Buku>>();

    // Path ke file data
    private static string filePath = "daftar_buku.json";

    static void Main(string[] args)
    {
        Console.WriteLine("=== Sistem Login ===");

        // Muat data buku dari file
        BacaDataBukuDariFile();

        // Login
        Console.Write("Masukkan username: ");
        string username = Console.ReadLine();

        Console.Write("Masukkan password: ");
        string password = Console.ReadLine();

        if (users.ContainsKey(username) && users[username].Password == password)
        {
            Console.WriteLine($"Login berhasil! Selamat datang, {username}.");
            string role = users[username].Role;

            if (role == "Admin")
            {
                MenuAdmin();
            }
            else if (role == "User")
            {
                if (!peminjamanBuku.ContainsKey(username))
                {
                    peminjamanBuku[username] = new List<Buku>(); // Inisialisasi peminjaman untuk User
                }
                MenuUser(username);
            }
        }
        else
        {
            Console.WriteLine("Username atau password salah.");
        }

        Console.WriteLine("Program selesai.");

        // Simpan data buku sebelum keluar
        SimpanDataBukuKeFile();
    }

    // Menu untuk Admin
    static void MenuAdmin()
    {
        while (true)
        {
            Console.WriteLine("\n=== Menu Admin ===");
            Console.WriteLine("1. Lihat Daftar Buku");
            Console.WriteLine("2. Tambah Buku");
            Console.WriteLine("3. Hapus Buku");
            Console.WriteLine("4. Update Buku");
            Console.WriteLine("5. Logout");
            Console.Write("Pilih menu: ");
            string pilihan = Console.ReadLine();
            SimpanDataBukuKeFile();

            switch (pilihan)
            {
                case "1":
                    LihatDaftarBuku();
                    break;
                case "2":
                    TambahBuku();
                    break;
                case "3":
                    HapusBuku();
                    break;
                case "4":
                    UpdateBuku();
                    break;
                case "5":
                    Console.WriteLine("Logout berhasil.");
                    return;
                default:
                    Console.WriteLine("Pilihan tidak valid. Coba lagi.");
                    break;
            }
        }
    }

    // Menu untuk User
    static void MenuUser(string username)
    {
        while (true)
        {
            Console.WriteLine("\n=== Menu User ===");
            Console.WriteLine("1. Cari Buku");
            Console.WriteLine("2. Filter Buku");
            Console.WriteLine("3. Lihat Daftar Buku");
            Console.WriteLine("4. Pinjam Buku");
            Console.WriteLine("5. Lihat Buku yang Dipinjam");
            Console.WriteLine("6. Kembalikan Buku");
            Console.WriteLine("7. Logout");
            Console.Write("Pilih menu: ");
            string pilihan = Console.ReadLine();
            SimpanDataBukuKeFile();


            switch (pilihan)
            {
                case "1":
                    CariBuku(username);
                    break;
                case "2":
                    FilterBuku();
                    break;
                case "3":
                    LihatDaftarBuku();
                    break;
                case "4":
                    PinjamBuku(username);
                    break;
                case "5":
                    LihatBukuDipinjam(username);
                    break;
                case "6":
                    KembalikanBuku(username);
                    break;
                case "7":
                    SimpanDataBukuKeFile();
                    Console.WriteLine("Logout berhasil.");
                    return;
                default:
                    Console.WriteLine("Pilihan tidak valid. Coba lagi.");
                    break;
            }
        }
    }

    // Fungsi untuk mencari buku berdasarkan kata kunci
    static void CariBuku(string username)
    {
        Console.WriteLine("\n=== Cari Buku ===");
        Console.WriteLine("Masukkan judul buku yang ingin dicari : ");
        string keyword = Console.ReadLine().ToLower();

        Console.WriteLine("\nHasil Pencarian Buku:");
        var hasilPencarian = daftarBuku.FindAll(buku => buku.Title.ToLower().Contains(keyword) || buku.Author.ToLower().Contains(keyword));

        if (hasilPencarian.Count == 0)
        {
            Console.WriteLine("Tidak ada buku yang sesuai dengan judul yang dimaksud.");
        }
        else
        {
            for (int i = 0; i < hasilPencarian.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {hasilPencarian[i]}");
            }
        }
    }

    // Fungsi untuk memfilter buku berdasarkan genre
    static void FilterBuku()
    {
        Console.WriteLine("\n=== Filter Buku ===");
        Console.Write("Masukkan genre buku : ");
        string genre = Console.ReadLine()?.ToLower();

        Console.WriteLine("\nHasil Filter Berdasarkan Genre :");
        var hasilFilter = daftarBuku.FindAll(buku => buku.Genre.ToLower().Contains(genre));

        if (hasilFilter.Count == 0)
        {
            Console.WriteLine("Tidak ada buku dengan genre yang dimaksud.");
        }
        else
        {
            for (int i = 0; i < hasilFilter.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {hasilFilter[i]}");
            }
        }
    }


    // Fungsi untuk melihat daftar buku
    static void LihatDaftarBuku()
    {
        Console.WriteLine("\n=== Daftar Buku ===");
        if (daftarBuku.Count == 0)
        {
            Console.WriteLine("Tidak ada buku tersedia.");
            return;
        }

        for (int i = 0; i < daftarBuku.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {daftarBuku[i]}");
        }
    }


    // Fungsi untuk menambah buku (hanya Admin)
    static void TambahBuku()
    {
        Console.WriteLine("\n=== Tambah Buku ===");
        Console.Write("\nMasukkan judul buku baru : ");
        string judulBuku = Console.ReadLine();

        Console.Write("Masukkan penulis buku baru : ");
        string penulisBuku = Console.ReadLine();

        Console.Write("Masukkan genre buku baru : ");
        string genreBuku = Console.ReadLine();

        Console.Write("Masukkan tahun rilis buku baru : ");
        int tahunRilisBuku = Convert.ToInt32(Console.ReadLine());

        Console.Write("Masukkan deskripsi singkat : ");
        string deskripsiBuku = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(judulBuku))
        {
            var bukuBaru = new Buku
            {
                Title = judulBuku,
                Author = penulisBuku,
                Genre = genreBuku,
                Year = tahunRilisBuku,
                Description = deskripsiBuku
            };

            daftarBuku.Add(bukuBaru);
            SimpanDataBukuKeFile(); // Simpan perubahan
            Console.WriteLine($"Buku \"{judulBuku}\" berhasil ditambahkan.");
        }
        else
        {
            Console.WriteLine("Judul buku tidak boleh kosong.");
        }
    }


    // Fungsi untuk menghapus buku (hanya Admin)
    static void HapusBuku()
    {
        Console.WriteLine("\n=== Hapus Buku ===");
        LihatDaftarBuku();
        Console.Write("\nMasukkan nomor buku yang ingin dihapus : ");
        if (int.TryParse(Console.ReadLine(), out int nomor) && nomor > 0 && nomor <= daftarBuku.Count)
        {
            var bukuDihapus = daftarBuku[nomor - 1];
            daftarBuku.RemoveAt(nomor - 1);
            SimpanDataBukuKeFile();
            Console.WriteLine($"Buku \"{bukuDihapus}\" berhasil dihapus.");
        }
        else
        {
            Console.WriteLine("Nomor buku tidak valid.");
        }
    }

    // Fungsi untuk memperbarui buku (hanya Admin)
    static void UpdateBuku()
    {
        Console.WriteLine("\n=== Update Buku ===");
        LihatDaftarBuku();
        Console.Write("\nMasukkan nomor buku yang ingin diperbarui : ");
        if (int.TryParse(Console.ReadLine(), out int nomor) && nomor > 0 && nomor <= daftarBuku.Count)
        {
            var buku = daftarBuku[nomor - 1];

            Console.Write("Masukkan judul baru (kosongkan jika tidak ingin mengubah) : ");
            string judulBaru = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(judulBaru)) buku.Title = judulBaru;

            Console.Write("Masukkan penulis baru (kosongkan jika tidak ingin mengubah) : ");
            string penulisBaru = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(penulisBaru)) buku.Author = penulisBaru;

            Console.Write("Masukkan genre baru (kosongkan jika tidak ingin mengubah) : ");
            string genreBaru = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(genreBaru)) buku.Genre = genreBaru;

            Console.Write("Masukkan tahun rilis baru (kosongkan jika tidak ingin mengubah) : ");
            string tahunInput = Console.ReadLine();
            if (int.TryParse(tahunInput, out int tahunBaru)) buku.Year = tahunBaru;

            Console.Write("Masukkan deskripsi baru (kosongkan jika tidak ingin mengubah) : ");
            string deskripsiBaru = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(deskripsiBaru)) buku.Description = deskripsiBaru;

            Console.WriteLine("Buku berhasil diperbarui.");
        }
        else
        {
            Console.WriteLine("Nomor buku tidak valid.");
        }
    }

    // Fungsi untuk meminjam buku (hanya User)
    static void PinjamBuku(string username)
    {
        bool berhasilPinjam = false; // Status apakah peminjaman berhasil

        while (!berhasilPinjam) // Loop sampai peminjaman berhasil
        {
            Console.WriteLine("\n=== Pinjam Buku ===");
            LihatDaftarBuku(); // Tampilkan daftar buku yang tersedia

            Console.Write("\nMasukkan nomor buku yang ingin dipinjam (atau ketik '0' untuk batal): ");
            if (int.TryParse(Console.ReadLine(), out int nomor))
            {
                if (nomor == 0)
                {
                    Console.WriteLine("Peminjaman dibatalkan.");
                    return; // Keluar dari fungsi jika pengguna memilih untuk batal
                }
                if (nomor > 0 && nomor <= daftarBuku.Count)
                {
                    var buku = daftarBuku[nomor - 1]; // Ambil buku berdasarkan nomor yang dipilih

                    // Minta data pengguna
                    Console.Write("Masukkan Nama Anda: ");
                    string namaPeminjam = Console.ReadLine();

                    Console.Write("Masukkan No Telepon Anda: ");
                    string noTelepon = Console.ReadLine();

                    // Validasi input nama dan telepon
                    if (string.IsNullOrWhiteSpace(namaPeminjam) || string.IsNullOrWhiteSpace(noTelepon))
                    {
                        Console.WriteLine("Nama atau No Telepon tidak boleh kosong. Silakan coba lagi.");
                        continue; // Kembali ke awal loop jika input tidak valid
                    }

                    // Membuat objek peminjaman
                    var peminjaman = new Peminjaman
                    {
                        BukuDipinjam = buku,
                        NamaPeminjam = namaPeminjam,
                        NoHP = noTelepon,
                        waktuPeminjaman = DateTime.Now
                    };

                    // Masukkan ke daftar peminjaman pengguna
                    if (!peminjamanBuku.ContainsKey(username))
                    {
                        peminjamanBuku[username] = new List<Buku>();
                    }

                    peminjamanBuku[username].Add(buku);
                    daftarBuku.Remove(buku); // Hapus buku dari daftar yang tersedia

                    Console.WriteLine($"Buku \"{buku.Title}\" berhasil dipinjam oleh {namaPeminjam} pada {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}");
                    berhasilPinjam = true; // Set status berhasil untuk keluar dari loop
                }
                else
                {
                    Console.WriteLine("Nomor buku tidak valid. Silakan coba lagi.");
                }
            }
        }
    }

    // Fungsi untuk melihat buku yang dipinjam (hanya User)
    static void LihatBukuDipinjam(string username)
    {
        Console.WriteLine("\n=== Buku yang Anda Pinjam ===");
        if (peminjamanBuku[username].Count == 0)
        {
            Console.WriteLine("Anda belum meminjam buku apa pun.");
        }
        else
        {
            int nomor = 1;
            foreach (var buku in peminjamanBuku[username])
            {
                Console.WriteLine($"{nomor}. {buku}");
                nomor++;
            }
        }
    }


    // Fungsi untuk mengembalikan buku (hanya User)
    static void KembalikanBuku(string username)
    {
        Console.WriteLine("\n=== Kembalikan Buku ===");
        LihatBukuDipinjam(username);

        while (true)
        {
            Console.Write("\nMasukkan nomor buku yang ingin dikembalikan (atau ketik '0' untuk batal): ");
            if (int.TryParse(Console.ReadLine(), out int nomor))
            {
                if (nomor == 0)
                {
                    Console.WriteLine("Pengembalian dibatalkan.");
                    return; // Keluar dari fungsi
                }

                if (nomor > 0 && nomor <= peminjamanBuku[username].Count)
                {
                    var bukuDikembalikan = peminjamanBuku[username][nomor - 1];
                    peminjamanBuku[username].RemoveAt(nomor - 1);
                    daftarBuku.Add(bukuDikembalikan);
                    SimpanDataBukuKeFile(); // Simpan perubahan ke file

                    DateTime waktuPengembalian = DateTime.Now;
                    Console.WriteLine($"Buku \"{bukuDikembalikan.Title}\" berhasil dikembalikan pada {waktuPengembalian:dddd, dd MMMM yyyy HH:mm:ss}.");
                }
                else
                {
                    Console.WriteLine("Nomor buku tidak valid. Silakan coba lagi.");
                    KembalikanBuku(username); // Panggil ulang fungsi untuk mencoba kembali
                }
            }
            else
            {
                Console.WriteLine("Input tidak valid. Silakan masukkan nomor yang benar.");
                KembalikanBuku(username); // Panggil ulang fungsi untuk mencoba kembali
            }
        }
    }



    // Membaca data buku dari file
    static void BacaDataBukuDariFile()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                daftarBuku = JsonConvert.DeserializeObject<List<Buku>>(json) ?? new List<Buku>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Terjadi kesalahan saat membaca data dari file: {ex.Message}");
            daftarBuku = new List<Buku>(); // Gunakan daftar kosong jika terjadi error
        }
    }


    // Menyimpan data buku ke file
    static void SimpanDataBukuKeFile()
    {
        string json = JsonConvert.SerializeObject(daftarBuku, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

}

class Buku
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return $"{Title} oleh {Author} ({Year}) - Genre: {Genre}\n{Description}";
    }
}

class Peminjaman
{
    public Buku BukuDipinjam { get; set; }
    public string NamaPeminjam { get; set; }
    public string NoHP { get; set; }
    public DateTime waktuPeminjaman { get; set; }
    public DateTime TanggalPengembalian { get; set; }
}


