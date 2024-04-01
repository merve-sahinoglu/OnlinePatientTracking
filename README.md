# OnlinePatientTracking

Servislerde bulunan Dockerfile dosyaları kullanılarak Dockerize işlemleri tamamlanır.
Daha önceden kurulu bulunan RabbitMQ ve SQL bağlantıları için her servisin içinde bulunan appsettings.json dosyası düzenlenir. RabbitMQ için proje içerisindeki Comon içindeki Evenbus.RabbitMQ içinden conf değiştirilir., SQL için    "ConnectionStrings": , alanı düzenlenir.
Uygulama ayağa kaldırılır, migration için servisler ayağa kalkarken Database ve tablolar otomatik olarak oluşturulur.
Kullanıma hazır!
