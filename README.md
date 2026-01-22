# 🌀 Rush Roll – Scripts del Proyecto Final DAM

**Rush Roll** es un videojuego desarrollado en Unity.  
Este repositorio contiene los scripts principales que gestionan la lógica del juego, incluyendo físicas, personalización visual, progresión de niveles, persistencia de datos con Firebase, servicios de Google Play Games (GPGS), compras dentro de la aplicación y publicidad con mediación.

---

## 🧩 Funcionalidades destacadas

- 🎯 **Control de juego**
  - Movimiento de la bola mediante osciloscopio, movimiento de cámara, salto y colisiones.
  - Gestión de obstáculos y objetos interactivos.
  - Sistema de progresión por niveles y desbloqueo de logros.

- 🛍 **Personalización**
  - Sistema de skins desbloqueables con monedas.
  - Menú de tienda para seleccionar y comprar skins.
  - Compras dentro de la aplicación (IAP) para contenido no consumible (skins) y consumible(monedas).

- 💾 **Persistencia de datos**
  - Guardado en Firebase si hay conexión, si no en Player Prefs de:
    - Perfil del jugador
    - Monedas y progreso en niveles
    - Skins desbloqueadas
    - Rankings globales

- 🔐 **Seguridad – Firebase App Check**
  - Protección del acceso a los servicios de Firebase frente a usos no autorizados mediante AppCheck junto con Play Integrity API
  - Verificación de que las solicitudes provienen de una app legítima
  - Verificación tanto de usuarios como de acceso a Firestone para mayor seguridad
  - Debug Provider habilitado únicamente en entorno de desarrollo
  - Prevención de accesos desde apps modificadas o entornos no confiables

- 🕹 **Integración con Google Play Games Services (GPGS)**
  - Inicio de sesión automático con Google Play
  - Logros (achievements) y leaderboards
  - Acceso al nombre de usuario e imagen del jugador
  - In App Purhase

- 🔊 **Gestión de experiencia de usuario**
  - `AudioManager` para control de música y efectos de sonido
  - `VibrationManager` para retroalimentación háptica
  - Menús, UI y ajustes con animaciones usando LeanTween

- 📢 **Monetización y anuncios**
  - Integración de publicidad mediante **Unity Ads** y **Google AdMob** con mediación usando LevelPlay
  - Gestión de intersticiales, rewarded y banners
  - Soporte de compras dentro de la aplicación para desbloquear contenido o ventajas

- 📝 **Reseñas y Feedback**
  - Botón en el menú de ajustes para dejar una reseña en Google Play.
  - In App Review, permite al usuario valorar la aplicación sin salir del juego.

---

## 🛠️ Tecnologías utilizadas

- **Unity** (motor de juego)
- **C#** (lenguaje de programación)
- **Firebase Realtime Database y Firestore** (persistencia de datos)
- **Firebase App Check con Play Integrity API** (protección frente a accesos no autorizados)
- **Google Play Games Services (GPGS)** (logros, leaderboards, autenticación)
- **LevelPlay** (Mediador de anuncios de Unity Ads y AdMob)
- **In-App Purchases (IAP)** (compras dentro de la app)
- **In-App Review** (Calificación sin salir de la App)
- **Arquitectura modular** y patrones **Singleton** para managers persistentes

---

## 📂 Estructura de la carpeta `scripts`

- `Animals/` – Lógica de comportamiento de objetos animales
- `GameObjects/` – Scripts de objetos interactivos y físicas
- `Global/` – Managers globales
- `Level/` – Gestión de niveles
- `LevelSelector/` - Selección de niveles
- `LocalSave/` - Guardado local
- `LogIn` - Inicio de sesión
- `Menu/` – Scripts de menús y animaciones de UI
- `Obstacles/` – Lógica de obstáculos y generadores
- `Player/` – Control de la bola y cámara
- `Ranking/` - Gestión del ranking en firebase
- `Settings/` – Gestión de ajustes y preferencias
- `Shop/` – Lógica de tienda y personalización, incluyendo compras IAP
- `UI/` - Gestión de la UI


---

## 📜 Licencia

Este proyecto está bajo la licencia **MIT**.  
Puedes usar, modificar y distribuir el código libremente, siempre que se mantenga la atribución al autor.

---

## 👤 Autor

**Pedro Javier Morales Leyva**  
Estudiante de DAM | Proyecto Final 2025/2026