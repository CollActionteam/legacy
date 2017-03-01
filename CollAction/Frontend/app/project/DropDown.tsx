import * as React from "react";

const Options = (list) => <div className="drop-down-options"></div>;

interface IDropdownProps {
  options: string[];
}

interface IDropdownState {
  open: boolean;
}

export default class DropDown extends React.Component<IDropdownProps, IDropdownState> {
  constructor (props) {
    super(props);
    this.state = { open: false };
  }

  onClick(...args) {
    this.setState({open: !this.state.open});
  }

  render () {
    return (
      <div onClick={() => this.onClick()} className="project-filter-drop-down">
        {this.props.options[0]}
        {this.state.open ? <i className="fa fa-chevron-up" /> : <i className="fa fa-chevron-down" />}
        {this.state.open ? <Options list={this.props.options} /> : null}
      </div>
    );
  }
}
