# EcoSys - Evolutionary Ecosystem Simulation Framework

A production-ready, highly complex ecosystem simulation framework built in Unity using C#. This project serves as my Master's Thesis, focusing on the comparative analysis of advanced Artificial Intelligence paradigms (FSM, Fuzzy Cognitive Maps, Utility AI) paired with genetic algorithms, procedural generation, and real-time telemetry tracking.

## Core Core Engineering Pillars

- **Multi-Paradigm AI Decision Making**: Fully decoupled and modular AI controller system allowing agents to switch between Classic Finite State Machines (FSM), Fuzzy Cognitive Maps (FCM), and Utility-Based AI for behavior evaluation.
- **Genetic Algorithms & Natural Selection**: Realistic biological simulation including dynamic status tracking (hunger, thirst, stamina, mating urge), custom genetic crossover (breeding), and adjustable mutation rates influencing neural weights.
- **Procedural Terrain & World Generation**: Runtime generation of 3D endless terrain utilizing layered noise maps, custom fall-off graphs, dynamic mesh deformation, and multi-biome texture generation.
- **Production-Grade Telemetry & Monitoring**: Out-of-the-box support for runtime metric logging. Real-time simulation statistics are streamed to an time-series InfluxDB database and visualized using Grafana, orchestrated via Docker Compose.
- **Custom Graphics Pipelines**: Tailored stylization using Unity’s Universal Render Pipeline (URP) with custom ShaderGraphs for dynamic depth-faded height gradients and stylized waters.

## Architecture & Project Structure

- `Assets/Scripts/AI/`: Core decision engines (FSM, FCM, Utility) and genetic evolutionary algorithms.
- `Assets/Scripts/World/`: Procedural mesh, terrain noise, and water generation systems.
- `Assets/Scripts/Animals/`: Agent behavioral logic, component state handling, and sensors.
- `EcoSys-monitoring/`: Docker infrastructure for Grafana and InfluxDB time-series telemetry.

## Tech Stack

- **Engine**: Unity 3D
- **Language**: C# (.NET / Unity Scripting API)
- **Render Pipeline**: Universal Render Pipeline (URP) & ShaderGraph
- **DevOps & Analytics**: Docker, Docker Compose, InfluxDB, Grafana

---

# EcoSys - Evolúciós Ökoszisztéma Szimulációs Keretrendszer

Egy C# nyelven, Unity motorral fejlesztett, rendkívül összetett ökoszisztéma-szimulációs keretrendszer. Ez a projekt a diplomamunkám (Master's Thesis), amely a fejlett mesterséges intelligencia paradigmák (FSM, Fuzzy Cognitive Maps, Utility AI) összehasonlító elemzésére fókuszál, kiegészítve genetikus szoftveres algoritmusokkal, procedurális világ generálással és valós idejű telemetria-követéssel.

## Főbb Fejlesztői és Mérnöki Pillérek

- **Több-paradigmás MI döntéshozatal**: Teljesen leválasztott, moduláris MI vezérlőrendszer, amely lehetővé teszi az ágensek számára a Klasszikus Véges Állapotú Gép (FSM), a Fuzzy Kognitív Térkép (FCM) és a Hasznossági Alapú MI (Utility AI) közötti váltást.
- **Genetikus algoritmusok és természetes szelekció**: Valósághű biológiai szimuláció egyedi tulajdonság-öröklődéssel (szaporodás), mutációs rátákkal, valamint az ágensek állapotainak (éhség, szomjúság, állóképesség, párzási vágy) folyamatos monitorozásával.
- **Procedurális terep- és világ generálás**: Háromdimenziós domborzat futásidejű előállítása rétegzett zajfüggvények (Noise), fall-off térképek, dinamikus mesh-deformáció és több-biomos textúragenerálás segítségével.
- **Ipari szintű telemetria és monitorozás**: Beépített idősoros adatgyűjtés. A szimuláció statisztikái valós időben streamelődnek egy InfluxDB adatbázisba, amely Grafana dashboardon vizualizálható, Docker Compose segítségével orchestrálva.
- **Egyedi grafikus csővezeték**: Egyedi vizuális stílus a Unity Universal Render Pipeline (URP) és saját ShaderGraph-ok (mélységfakuló magassági gradiensek, stilizált víz) használatával.

## Architektúra és Projektstruktúra

- `Assets/Scripts/AI/`: Döntési motorok (FSM, FCM, Utility) és evolúciós genetikai algoritmusok.
- `Assets/Scripts/World/`: Procedurális háló (mesh), zajgenerátorok és spawner rendszerek.
- `Assets/Scripts/Animals/`: Ágensek viselkedési logikája, állapotkezelés és szenzorok.
- `EcoSys-monitoring/`: Docker infrastruktúra a Grafana és InfluxDB telemetriához.