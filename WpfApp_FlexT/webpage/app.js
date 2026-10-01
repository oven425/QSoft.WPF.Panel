/*
  Lays out #box from the FlexPanelTestData (WpfApp_FlexT\MainWindow.xaml.cs) that WPF posts with
  CoreWebView2.PostWebMessageAsJson, so CSS flexbox can be compared with QSoft.WPF.Panel.FlexPanel.
*/
(() => {
  'use strict';

  // FlexPanel enums in declaration order, so both enum names and numeric values are understood.
  const cssFlexDirection = {
    Row: 'row',
    RowReverse: 'row-reverse',
    Column: 'column',
    ColumnReverse: 'column-reverse',
  };
  const cssJustifyContent = {
    Start: 'flex-start',
    End: 'flex-end',
    Center: 'center',
    SpaceAround: 'space-around',
    SpaceBetween: 'space-between',
    SpaceEvenly: 'space-evenly',
  };
  const cssAlignItems = {
    Start: 'flex-start',
    End: 'flex-end',
    Center: 'center',
    Stretch: 'stretch',
  };

  const box = document.getElementById('box');
  const viewport = document.getElementById('viewport');
  const itemTemplate = document.getElementById('item-template');

  // Reads a property by its C# name; the camelCase name is accepted too.
  function read(source, name) {
    if (source === null || typeof source !== 'object') return undefined;
    return source[name] ?? source[name[0].toLowerCase() + name.slice(1)];
  }

  // Unknown values fall back to the first enum member, which is the FlexPanel default.
  function toCss(map, value) {
    const names = Object.keys(map);
    const name = typeof value === 'number'
      ? names[value]
      : names.find(n => n.toLowerCase() === String(value).toLowerCase());
    return map[name ?? names[0]];
  }

  // FlexPanel treats negative values as 0.
  function toNumber(value) {
    const number = Number(value);
    return Number.isFinite(number) ? Math.max(number, 0) : 0;
  }

  function createItem(child, index) {
    const element = itemTemplate.content.firstElementChild.cloneNode(true);
    element.querySelector('.item-label').textContent = `index: ${index}`;
    const basis = toNumber(read(child, 'Basis'));
    Object.assign(element.style, {
      flexGrow: String(toNumber(read(child, 'Grow'))),
      flexShrink: String(toNumber(read(child, 'Shrink'))),
      // FlexPanel keeps the child's desired size when Basis is 0.
      flexBasis: basis > 0 ? `${basis}px` : 'auto',
    });
    return element;
  }

  // ScrollViewer measures its content without a limit along a scrollable axis. FlexPanel answers with the total of
  // its items' flex base sizes plus gaps and padding, whereas CSS max-content (which Chromium computes without looking
  // at flex-basis) would let the items shrink. So the items are laid out once in an oversized box, without growing or
  // shrinking, to read that total.
  function fitMainAxisToItems(isRow, paddingBefore, paddingAfter, gap) {
    const size = isRow ? 'width' : 'height';
    const items = Array.from(box.children);
    const flex = items.map(el => [el.style.flexGrow, el.style.flexShrink]);
    box.style[size] = '1000000px';
    for (const el of items) {
      el.style.flexGrow = '0';
      el.style.flexShrink = '0';
    }
    let total = paddingBefore + paddingAfter + gap * Math.max(items.length - 1, 0);
    for (const el of items) total += el.getBoundingClientRect()[size];
    items.forEach((el, i) => {
      [el.style.flexGrow, el.style.flexShrink] = flex[i];
    });
    box.style[size] = `${total}px`;
  }

  function render(data, viewportSize) {
    const padding = read(data, 'Padding');
    const parent = read(data, 'Parent');
    const scrollX = Boolean(read(parent, 'EnableHorizontalScrollbar'));
    const scrollY = Boolean(read(parent, 'EnableVerticalScrollbar'));
    const direction = toCss(cssFlexDirection, read(data, 'Direction'));
    const isRow = direction.startsWith('row');
    const gap = toNumber(read(data, 'Gap'));
    const paddingLeft = toNumber(read(padding, 'Left'));
    const paddingTop = toNumber(read(padding, 'Top'));
    const paddingRight = toNumber(read(padding, 'Right'));
    const paddingBottom = toNumber(read(padding, 'Bottom'));
    // Same size WPF's ScrollViewer gave the FlexPanel; the WebView2 control's own (integer-rounded) size is not used.
    const viewportWidth = toNumber(read(viewportSize, 'Width'));
    const viewportHeight = toNumber(read(viewportSize, 'Height'));
    Object.assign(viewport.style, {
      overflowX: scrollX ? 'auto' : 'hidden',
      overflowY: scrollY ? 'auto' : 'hidden',
      width: viewportWidth > 0 ? `${viewportWidth}px` : '',
      height: viewportHeight > 0 ? `${viewportHeight}px` : '',
    });
    Object.assign(box.style, {
      // Mirrors how ScrollViewer measures its content: an axis with the scrollbar disabled constrains the
      // panel to the viewport (items shrink to fit), a scrollable axis lets the panel grow to its content
      // (but never smaller than the viewport).
      width: scrollX ? 'max-content' : 'auto',
      minWidth: '100%',
      height: scrollY ? 'auto' : '100%',
      minHeight: '100%',
      flexDirection: direction,
      justifyContent: toCss(cssJustifyContent, read(data, 'JustifyContent')),
      alignItems: toCss(cssAlignItems, read(data, 'AlignItems')),
      gap: `${gap}px`,
      paddingLeft: `${paddingLeft}px`,
      paddingTop: `${paddingTop}px`,
      paddingRight: `${paddingRight}px`,
      paddingBottom: `${paddingBottom}px`,
    });

    const childs = read(data, 'Childs');
    box.replaceChildren(...(Array.isArray(childs) ? childs : []).map(createItem));

    if (isRow ? scrollX : scrollY) {
      fitMainAxisToItems(
        isRow,
        isRow ? paddingLeft : paddingTop,
        isRow ? paddingRight : paddingBottom,
        gap);
    }
  }

  // Measures each rendered item relative to #box's own top-left (i.e. inside its padding), the
  // same coordinate space MainWindow.xaml.cs uses when it measures FlexPanel's children relative
  // to the FlexPanel itself. Both #box and the WPF FlexPanel have no border/margin of their own,
  // so this lines up directly with TranslatePoint/ActualWidth/ActualHeight on the WPF side.
  function measureItems() {
    const boxRect = box.getBoundingClientRect();
    return Array.from(box.children).map(el => {
      const r = el.getBoundingClientRect();
      return {
        x: r.left - boxRect.left,
        y: r.top - boxRect.top,
        width: r.width,
        height: r.height,
      };
    });
  }

  // Size of #box itself, so WPF can tell whether both renderers laid the items out in equally sized containers.
  function measureContainer() {
    const r = box.getBoundingClientRect();
    return { x: 0, y: 0, width: r.width, height: r.height };
  }

  // Echoes measurements back to WPF so MainWindow.xaml.cs can compare them against the WPF
  // FlexPanel's own layout for the same RequestId (see ApplyTestCase/OnWebMessageReceived).
  function postMeasurements(requestId) {
    if (!window.chrome?.webview) return;
    window.chrome.webview.postMessage({
      type: 'measurements',
      requestId,
      items: measureItems(),
      container: measureContainer(),
    });
  }

  // PostWebMessageAsJson delivers an object, PostWebMessageAsString delivers JSON text.
  window.chrome?.webview?.addEventListener('message', event => {
    let data = event.data;
    if (typeof data === 'string') {
      try {
        data = JSON.parse(data);
      } catch {
        return;
      }
    }
    if (data === null || typeof data !== 'object') return;

    // WPF posts { RequestId, Viewport, TestData } (see MainWindow.xaml.cs's ShowInWebView/WebPageEnvelope).
    const requestId = read(data, 'RequestId');
    const testData = read(data, 'TestData');
    if (testData === undefined) return;

    render(testData, read(data, 'Viewport'));
    // Wait a frame so layout/paint for the new render has settled before measuring it.
    requestAnimationFrame(() => postMeasurements(requestId));
  });
})();
