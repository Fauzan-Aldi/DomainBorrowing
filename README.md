# PeminjamanDomainC

Peminjaman Domain adalah metode baru untuk menyembunyikan lalu lintas  menggunakan CDN. Ini pertama kali dipresentasikan di Blackhat Asia 2021 oleh [Junyu Zhou](https://twitter.com/md5_salt) dan Tianze Ding. Anda dapat menemukan slide presentasi [di sini](https://www.blackhat.com/asia-21/briefings/schedule/#domain-borrowing-catch-my-c-traffic-if-you-can-22314) dan [di sini](https://i.blackhat.com/asia-21/Thursday-Handouts/as-21-Ding-Domain-Borrowing-Catch-My-C2-Traffic-If-You-Can.pdf).

DomainBorrowing dibuat sebagai bagian dari magang di Tim Merah [NVISO Security](https://nviso.eu/en). Ikuti pekerjaan mereka di [blog mereka](https://blog.nviso.eu) dan [Twitter](https://twitter.com/NVISO_Labs).

DomainBorrowing adalah sebuah ekstensi untuk Cobalt Strike yang ditulis dalam bahasa C# menggunakan [Spesifikasi C2 Eksternal] Cobalt Strike (https://www.cobaltstrike.com/help-externalc2). Ekstensi ini didasarkan pada pustaka [ExternalC2](https://twitter.com/ryhanson) milik [Ryan Hanson](https://github.com/ryhanson/ExternalC2) dan [Covenant PoC](https://github.com/Dliv3/DomainBorrowing) yang disediakan dalam slide Blackhat Asia 2021.

Saya menulis [blogpost](https://cerbersec.com/2021/05/18/domain-borrowing.html) tentang hal itu.

## Client
Proyek ClientC2 bertanggung jawab untuk menyambung ke CDN dan meminta stager dari Server. Proyek ini mengelola komunikasi antara Beacon dan ServerC2.

Konfigurasi untuk klien dilakukan di `Program.cs`. Klien membutuhkan 4 parameter:
1. domain atau alamat ip untuk mencapai server tepi CDN
2. SNI
3. Port opsional untuk berkomunikasi dengan CDN, port default adalah 443
4. 4. Tidur opsional dalam milidetik di antara pesan, standarnya adalah 60 detik

```csharp
Klien klien = new Klien(“target.domain.or.ip.address.here”, “target.sni.here”, 443, 60000);
```

## Server
Proyek Server bertanggung jawab untuk meneruskan komunikasi antara CDN dan Teamserver Cobalt Strike melalui soket ExternalC2.

Konfigurasi untuk server dilakukan di `SocketSettings.cs`. Tentukan alamat dan port pendengar ExternalC2 Cobalt Strike di sini.

```csharp
public SocketSettings()
{
    IpAddress = “127.0.0.1”;
    Port = “2222”;
}
```

Luncurkan server dengan: `sudo dotnet run --url http://127.0.0.1:80/`. Anda dapat menyesuaikan IP dan port sesuai dengan keinginan Anda dan mengonfigurasi CDN Anda dengan tepat.

## Masalah-masalah yang diketahui

* Server saat ini bergantung pada Client, jadi pastikan untuk menyalin proyek Client sebelum menjalankan Server.
