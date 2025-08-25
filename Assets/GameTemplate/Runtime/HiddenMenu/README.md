# Hidden Menu Module

Module này cung cấp chức năng hidden menu cho việc debug và testing trong game mobile.

## Tính năng chính

- **Trigger System**: Bấm button nhiều lần để mở hidden menu
- **Password Protection**: Xác thực password trước khi mở menu
- **Default UI**: Tự động tạo UI popup nếu không có prefab được assign
- **Debug Functions**: Các chức năng debug cơ bản (set level, currency, clear data)
- **Mobile Optimized**: UI được tối ưu cho mobile

## Cách sử dụng

### 1. Setup cơ bản

1. Tạo `HiddenMenuConfig` asset:
   - Right-click trong Project window
   - Create > GameTemplate > HiddenMenu > Config
   - Cấu hình password và các tính năng

2. Thêm `HiddenMenuManager` vào scene:
   - Tạo empty GameObject
   - Add component `HiddenMenuManager`
   - Assign config asset

### 2. Trigger Hidden Menu

#### Cách 1: Sử dụng HiddenMenuTrigger component
```csharp
// Gắn component này vào bất kỳ button nào
// Component sẽ tự động detect click và trigger hidden menu
```

#### Cách 2: Gọi trực tiếp từ code
```csharp
// Từ bất kỳ script nào
HiddenMenuManager.Instance.OnHiddenMenuTrigger();
```

### 3. Cấu hình

#### HiddenMenuConfig
- **Password**: Mật khẩu để mở hidden menu
- **Click Threshold**: Số lần click cần thiết (mặc định: 10)
- **Click Time Window**: Thời gian window để click (mặc định: 5 giây)
- **Debug Features**: Bật/tắt các tính năng debug
- **Currency Types**: Bật/tắt các loại tiền tệ

### 4. Các chức năng có sẵn

#### Default Debug Functions
- **Load Level**: Tải level theo số được nhập
- **Set Currency**: Set số lượng coins, gems, diamonds
- **Clear User Data**: Xóa toàn bộ dữ liệu người chơi
- **Performance Info**: Hiển thị thông tin hiệu suất

#### Helper Functions
```csharp
// Set level
HiddenMenuHelper.SetPlayerLevel(5);

// Set currency
HiddenMenuHelper.SetCurrency("Coins", 1000);
HiddenMenuHelper.SetAllCurrencies(1000, 100, 50);

// Clear data
HiddenMenuHelper.ClearAllPlayerData();

// Get info
string deviceInfo = HiddenMenuHelper.GetDeviceInfo();
string perfInfo = HiddenMenuHelper.GetPerformanceInfo();
```

### 5. Custom UI

Nếu muốn sử dụng UI tùy chỉnh:

1. Tạo prefab với UI components
2. Assign prefab vào `HiddenMenuManager.hiddenMenuUIPrefab`
3. Prefab sẽ được sử dụng thay vì default UI

### 6. Events

Module phát ra các events:
- `"HiddenMenuOpened"`: Khi hidden menu được mở
- `"HiddenMenuClosed"`: Khi hidden menu được đóng

```csharp
// Listen to events
EventManager.Instance.AddListener<StringEvent>("HiddenMenuOpened", OnHiddenMenuOpened);
```

## Flow hoạt động

1. **Trigger**: Bấm button nhiều lần (>= threshold) trong time window
2. **Password Input**: Hiển thị màn hình nhập password
3. **Validation**: Kiểm tra password
4. **Menu**: Nếu đúng password, hiển thị hidden menu
5. **Functions**: Thực hiện các chức năng debug

## Bảo mật

- Password được lưu trong ScriptableObject (có thể encrypt thêm)
- Hidden menu chỉ hoạt động trong Development Build hoặc Editor
- Có thể disable hoàn toàn trong Production Build

## Tích hợp với hệ thống khác

Module được thiết kế để dễ dàng tích hợp với:
- LevelManager
- CurrencyManager  
- DataManager
- EventSystem

Chỉ cần uncomment và modify các dòng code tương ứng trong `HiddenMenuHelper`.

## Troubleshooting

### Hidden menu không mở
- Kiểm tra `HiddenMenuManager` có trong scene không
- Kiểm tra `HiddenMenuConfig` có được assign không
- Kiểm tra click count và time window

### UI không hiển thị
- Kiểm tra Canvas và EventSystem trong scene
- Kiểm tra TextMeshPro package có được import không

### Performance issues
- Disable `showDebugLogs` trong production
- Sử dụng custom UI prefab thay vì default UI 