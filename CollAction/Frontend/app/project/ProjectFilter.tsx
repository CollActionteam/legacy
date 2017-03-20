import * as React from "react";
import DropDown from "./DropDown";

const filterList = [ "all" ];
const statusList = [ "open", "closed", "funded" ];
const locations = [ "earth" ];

function onFilterChanged () {
  console.log('onChange');
}

export default () => (
  <div id="project-filter">
    Show me
    <DropDown onChange={onFilterChanged} options={filterList} />
    projects sorted on
    <DropDown onChange={onFilterChanged} options={locations}/>
    <br />
    which are
    <DropDown onChange={onFilterChanged} options={statusList} />
  </div>
);
