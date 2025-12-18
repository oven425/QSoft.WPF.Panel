# QSoft.WPF.Panel 
## Quick start
1. install from [nuget](https://www.nuget.org/packages/QSoft.WPF.Panel)
2. add qpanel into xaml
```xml
<Window x:Class="WpfApp_FlexPanelT.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp_FlexPanelT"
        mc:Ignorable="d"
        xmlns:qpanel="clr-namespace:QSoft.WPF.Panel;assembly=QSoft.WPF.Panel"
        Title="FlexPanel test site" Height="450" Width="800">

        <qpanel:FlexPanel>
            <Button Width="100">Test1</Button>
            <Button Width="100">Test2</Button>
            <Button Width="100">Test3</Button>
        </qpanel:FlexPanel>

</Window>
```
## Direction: Row,Column
```xml
<qpanel:FlexPanel FlexDirection="Row">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## JustifyContent: Start, End, Center, SpaceAround, SpaceBetween, SpaceEvenly
```xml
<qpanel:FlexPanel JustifyContent="Start">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## AlignItems: Start, End, Center, Stretch
```xml
<qpanel:FlexPanel AlignItems="Start">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## Gap
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button>Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## AlignSelf: Auto, Start, End, Center, Stretch
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button qpanel:FlexPanel.AlignSelf="Start">Test1</Button>
    <Button qpanel:FlexPanel.AlignSelf="End">Test2</Button>
    <Button qpanel:FlexPanel.AlignSelf="Center">Test3</Button>
</qpanel:FlexPanel>
```

## Grow
> use Grow, not set Width/Hieght when Direction Row/Column
```xml
<qpanel:FlexPanel FlexDirection="Row" Gap="8">
    <Button qpanel:FlexPanel.Grow="1">Test1</Button>
    <Button>Test2</Button>
    <Button>Test3</Button>
</qpanel:FlexPanel>
```

## Refrences
[Flexbox test](https://oven425.github.io/my-flex-app)


