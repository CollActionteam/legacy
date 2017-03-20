import * as React from "react";
import DropDown from "./DropDown";

const filterList = [ "all", "some" ];
const statusList = [ "open", "closed", "funded" ];
const locationList = [ "earth", "somewhere else" ];

function onFilterChanged () {
  console.log('onChange');
}

interface IProjectFilterProps {
  onChange: (projectFilterState: IProjectFilterState) => void;
}

export interface IProjectFilterState {
  filter: string;
  status: string;
  location: string;
}

export class ProjectFilter extends React.Component<IProjectFilterProps, IProjectFilterState> {

  constructor (props) {
    super(props);
    this.state = {
      filter: filterList[0],
      status: statusList[0],
      location: locationList[0],
    };
  }

  onChange(field: string, value: string) {
    const toUpdate = {};
    toUpdate[field] = value;
    this.setState(toUpdate);
    this.props.onChange({
      ...this.state,
      ...toUpdate,
    });
  }

  render () {
    return (
      <div id="project-filter">
        Show me
        <DropDown onChange={value => this.onChange("filter", value)} options={filterList} />
        projects sorted on
        <DropDown onChange={value => this.onChange("location", value)} options={locationList}/>
        <br />
        which are
        <DropDown onChange={value => this.onChange("status", value)} options={statusList} />
      </div>
    );
  }

}
