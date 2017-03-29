import * as React from "react";

const Options = ({ list, onSelected }) => {
  return (
    <div className="drop-down-options">
      <ul>
        {list.map(item => <li key={item.id} onClick={() => onSelected(item)}>{item.name}</li>)}
      </ul>
    </div>
  );
};

interface IDropdownProps {
  options: IDropDownListItem[];
  onChange: (selected: IDropDownListItem) => void;
}

interface IDropdownState {
  open: boolean;
  currentlySelected: IDropDownListItem;
}

export interface IDropDownListItem {
  id: string;
  name: string;
}

export default class DropDown extends React.Component<IDropdownProps, IDropdownState> {
  constructor (props) {
    super(props);
    this.state = { currentlySelected: null, open: false };
  }

  componentWillReceiveProps(nextProps) {
    if (!this.state.currentlySelected) {
      this.setState({ open: false, currentlySelected: nextProps.options[0] });
    }
  }

  onClick(...args) {
    this.setState({open: !this.state.open});
  }

  onSelected (option: IDropDownListItem) {
    this.setState({
      currentlySelected: option
    });

    if (this.state.currentlySelected.id !== option.id) {
      this.props.onChange(option);
    }
  }

  render () {
    return (
      <div onClick={() => this.onClick()} className="project-filter-drop-down">
        {this.state.currentlySelected ? this.state.currentlySelected.name : null }
        {this.state.open ? <i className="fa fa-chevron-up" /> : <i className="fa fa-chevron-down" />}
        {this.state.open ? <Options list={this.props.options} onSelected={(selection) => this.onSelected(selection)} /> : null}
      </div>
    );
  }
}
