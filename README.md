Gauntlet of the Hero (GOTH) - Full Stack Project
This project is a turn-based battle system featuring a Java Spring Boot backend and a Unity frontend. The architecture follows a clear separation of concerns, where the backend manages game logic and state, while the frontend handles visuals and user interaction.

System Architecture
Backend (Java Spring Boot)
Framework: Spring Boot 3.x

Logic: Manages battle snapshots, entity stats (Hero/Monster), and turn-based calculations.

API: RESTful endpoints for starting sessions, playing turns, and resolving encounters.

Frontend (Unity)
Engine: Unity 2022.3 LTS

Pattern: Event-driven UI that consumes the backend API.

Features: Reactive UI with Health Bars (Filled Image), dynamic log system, and procedural move button generation.

Getting Started
Prerequisites
Java: JDK 17 or higher.

Unity: Version 2022.3 LTS.

IDE: Visual Studio Code.

1. Running the Backend
Navigate to the /Backend_Spring_Boot folder.

Run the application using Maven:

Bash
./mvnw spring-boot:run
The server will start on http://localhost:8080.

2. Running the Frontend
Open Unity Hub and add the project from the /Frontend_Unity folder.

Open the scene located at Assets/Scenes/03_Battle.unity.

Ensure the NetworkManager (or equivalent) is pointing to http://localhost:8080.

Press Play.


Author
Milinko Miletić

Software Engineering Student @ University of Belgrade (ETF)
