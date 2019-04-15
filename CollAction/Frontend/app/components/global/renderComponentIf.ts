import * as ReactDOM from "react-dom";

export default function renderComponentIf(component, element) {
  if (element) {
    ReactDOM.render(component, element);
  }
}