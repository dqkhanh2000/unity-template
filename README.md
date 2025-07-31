# Kane Template Unity Package

A comprehensive Unity game development template that provides essential systems for building games quickly and efficiently.

## 🚀 Quick Start

### Install via Git URL
1. Open Unity Package Manager (Window > Package Manager)
2. Click the **+** button
3. Select **Add package from git URL**
4. Enter: `https://github.com/dqkhanh2000/unity-template.git?path=/Assets/KaneTemplate`
5. Click **Add**

### Install via Package Manager (if published)
1. Open Unity Package Manager
2. Search for "Kane Template"
3. Click **Install**

## 📦 Package Contents

### Core Systems
- **GameManager**: Central game state management
- **LevelManager**: Level loading and management
- **PlayerData**: Player data persistence
- **LevelData**: Level configuration system
- **GameEvents**: Event-driven architecture

### Event System
- **EventManager**: Centralized event management
- **IEvent & IEventListener**: Event interface system
- **MonoBehaviourEventListener**: Easy event listening for MonoBehaviours

### Editor Tools
- **Delete Example Folder**: Clean up after installation
- **Documentation Access**: Quick access to README files
- **About Dialog**: Package information

## 🛠️ Usage

```csharp
// Initialize the game manager
GameManager.Instance.Initialize();

// Subscribe to events
EventManager.Instance.Subscribe<GameStartEvent>(OnGameStart);

// Load a level
LevelManager.Instance.LoadLevel("Level1");
```

## 📚 Documentation

- [Core Systems](Assets/KaneTemplate/Core/README.md)
- [Event System](Assets/KaneTemplate/EventSystem/README.md)
- [Package README](Assets/KaneTemplate/README.md)

## 🎯 Features

- ✅ Game state management
- ✅ Level loading and management
- ✅ Player data persistence
- ✅ Event-driven architecture
- ✅ Editor tools for package management
- ✅ Comprehensive documentation
- ✅ Sample project included

## 🔧 Requirements

- Unity 2022.3 or higher
- TextMeshPro package

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](Assets/KaneTemplate/LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Changelog

See [CHANGELOG.md](Assets/KaneTemplate/CHANGELOG.md) for version history.

## 🆘 Support

If you encounter any issues or have questions, please:
1. Check the documentation
2. Search existing issues
3. Create a new issue with detailed information

---

**Created by Kane** - A Unity game development template for building better games faster. 