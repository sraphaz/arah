/**
 * Minimal deck-stage for handoff HTML decks (DC runtime x-import).
 * Shows one child section at a time; arrow keys / buttons navigate.
 * Registers window["deck-stage"] for component-from-global-scope.
 * Loaded via new Function("React","module","exports","require", src).
 */
function DeckStage(props) {
  const children = React.Children.toArray(props.children).filter(Boolean);
  const total = children.length;
  const [index, setIndex] = React.useState(0);
  const width = props.width || 1920;
  const height = props.height || 1080;

  React.useEffect(
    function () {
      function onKey(e) {
        if (e.key === "ArrowRight" || e.key === "PageDown" || e.key === " ") {
          e.preventDefault();
          setIndex(function (i) {
            return Math.min(i + 1, Math.max(total - 1, 0));
          });
        } else if (e.key === "ArrowLeft" || e.key === "PageUp") {
          e.preventDefault();
          setIndex(function (i) {
            return Math.max(i - 1, 0);
          });
        } else if (e.key === "Home") {
          setIndex(0);
        } else if (e.key === "End") {
          setIndex(Math.max(total - 1, 0));
        }
      }
      window.addEventListener("keydown", onKey);
      return function () {
        window.removeEventListener("keydown", onKey);
      };
    },
    [total]
  );

  if (total === 0) {
    return React.createElement(
      "div",
      {
        style: {
          width: "100%",
          height: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          color: "var(--fg-muted, #74806E)",
          fontFamily: "var(--font-sans, system-ui)",
        },
      },
      "Deck sem slides"
    );
  }

  const safeIndex = Math.min(index, total - 1);
  const slide = children[safeIndex];

  return React.createElement(
    "div",
    {
      role: "region",
      "aria-label": "Deck executivo",
      "aria-roledescription": "carousel",
      style: {
        position: "relative",
        width: "100%",
        height: "100%",
        minHeight: "100vh",
        overflow: "hidden",
        background: "var(--premium-bg, #0B0C0A)",
      },
    },
    React.createElement(
      "div",
      {
        key: safeIndex,
        style: {
          width: "100%",
          height: "100%",
          minHeight: "100vh",
          position: "relative",
          aspectRatio: String(width) + " / " + String(height),
        },
      },
      slide
    ),
    React.createElement(
      "div",
      {
        style: {
          position: "fixed",
          right: 24,
          bottom: 24,
          zIndex: 20,
          display: "flex",
          alignItems: "center",
          gap: 10,
          fontFamily: "var(--font-sans, system-ui)",
          fontSize: 13,
          color: "var(--premium-fg2, #A6AC9E)",
          background: "rgba(14,16,13,0.82)",
          border: "1px solid rgba(255,255,255,0.12)",
          borderRadius: 999,
          padding: "8px 14px",
        },
      },
      React.createElement(
        "button",
        {
          type: "button",
          "aria-label": "Slide anterior",
          disabled: safeIndex === 0,
          onClick: function () {
            setIndex(function (i) {
              return Math.max(i - 1, 0);
            });
          },
          style: {
            border: "none",
            background: "transparent",
            color: "inherit",
            cursor: safeIndex === 0 ? "default" : "pointer",
            opacity: safeIndex === 0 ? 0.4 : 1,
            fontSize: 16,
            lineHeight: 1,
          },
        },
        "‹"
      ),
      React.createElement(
        "span",
        { "aria-live": "polite" },
        String(safeIndex + 1) + " / " + String(total)
      ),
      React.createElement(
        "button",
        {
          type: "button",
          "aria-label": "Próximo slide",
          disabled: safeIndex >= total - 1,
          onClick: function () {
            setIndex(function (i) {
              return Math.min(i + 1, total - 1);
            });
          },
          style: {
            border: "none",
            background: "transparent",
            color: "inherit",
            cursor: safeIndex >= total - 1 ? "default" : "pointer",
            opacity: safeIndex >= total - 1 ? 0.4 : 1,
            fontSize: 16,
            lineHeight: 1,
          },
        },
        "›"
      )
    )
  );
}

window["deck-stage"] = DeckStage;
module.exports = { "deck-stage": DeckStage };
