import * as React from "react";
import onClickOutside from "react-onclickoutside";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faChevronUp } from "@fortawesome/free-solid-svg-icons";
import { faChevronDown } from "@fortawesome/free-solid-svg-icons";

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

class DropDown extends React.Component<IDropdownProps, IDropdownState> {
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
    this.setState({
      open: !this.state.open,
    });
  }

  handleClickOutside = () => {
    if (this.state.open) {
      this.setState({
        open: false,
      });
    }
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
        {this.state.open ? <FontAwesomeIcon icon={faChevronUp} /> : <FontAwesomeIcon icon={faChevronDown} /> }
        {this.state.open ? <Options list={this.props.options} onSelected={(selection) => this.onSelected(selection)} /> : null}
      </div>
    );
  }
}

export default onClickOutside(DropDown);
