import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "./renderComponentIf";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faMapMarkerAlt } from "@fortawesome/free-solid-svg-icons";

library.add(faMapMarkerAlt);

class MapMarkerItem extends React.Component {
  render() {
    return (
      <div className="">
        <FontAwesomeIcon icon={faMapMarkerAlt} />
      </div>
    );
  }
}

renderComponentIf(
  <MapMarkerItem />,
  document.getElementById("icon-map-marker")
);
