import * as React from "react";

const Options = ({ list, onSelected }) => {
  return (
    <div className="drop-down-options">
      <ul>
        {list.map(item => <li key={item} onClick={() => onSelected(item)}>{item}</li>)}
      </ul>
    </div>
  );
};

interface IDropdownProps {
  options: string[];
  onChange: (selected: string) => void;
}

interface IDropdownState {
  open: boolean;
  currentlySelected: string;
}

export default class DropDown extends React.Component<IDropdownProps, IDropdownState> {
  constructor (props) {
    super(props);
    this.state = { open: false, currentlySelected: this.props.options[0] };
  }

  onClick(...args) {
    this.setState({open: !this.state.open});
  }

  onSelected (option: string) {
    this.setState({
      currentlySelected: option
    });

    if (this.state.currentlySelected !== option) {
      this.props.onChange(option);
    }
  }

  render () {
    return (
      <div onClick={() => this.onClick()} className="project-filter-drop-down">
        {this.state.currentlySelected}
        {this.state.open ? <i className="fa fa-chevron-up" /> : <i className="fa fa-chevron-down" />}
        {this.state.open ? <Options list={this.props.options} onSelected={(selection) => this.onSelected(selection)} /> : null}
      </div>
    );
  }
}
