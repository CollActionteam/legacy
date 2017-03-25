import * as React from "react";
import * as ReactDOM from "react-dom";

interface CarouselItem {
  name: string;
  text: string;
}

interface ILandscapeCarouselProps {
  items: CarouselItem[];
}

interface ILandscapeCarouselState {
  selected: number;
}

const Item = (props) => {
  return (
    <li onClick={props.onClick} className={props.isSelected ? "selected" : ""}>
       <div className="coll-action-dot-point">{props.stepCount}</div>
       {props.itemTitle}
    </li>
  );
}

export default class LandscapeCarousel extends React.Component<ILandscapeCarouselProps, ILandscapeCarouselState> {

  constructor(props) {
    super(props);
    this.state = { selected: 0 };
  }

  onItemClicked(index) {
    this.setState({selected: index});
  }

  render () {
    return (
      <div>
        <div className="col-md-4 col-md-offset-2 col-xs-12 landscape-carousel-select">
          <p>In 6 easy Steps</p>
          <ul>
            {this.props.items.map((item, index) => <Item key={index + 1}
              isSelected={index === this.state.selected}
              stepCount={index + 1}
              onClick={() => this.onItemClicked(index)}
              itemTitle={item.name} />)}
          </ul>
        </div>

        <div className="col-xs-4 landscape-carousel-body">
          <p>{this.props.items[this.state.selected].text}</p>
        </div>

      </div>
    );
  }
}
