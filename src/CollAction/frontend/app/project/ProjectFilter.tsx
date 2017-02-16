import * as React from "react";
import DropDown from "./DropDown";

const filterList = [ "all" ];
const statusList = [ "open" ];
const locations = [ "earth" ];

export default () => (
  <div id="project-filter">
    Show me <DropDown options={filterList} /> projects sorted on <DropDown options={locations}/>  <br />
    which are <DropDown options={statusList} />
  </div>
);
