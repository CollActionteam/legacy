import * as React from "react";
import * as ReactDOM from "react-dom";

interface ICarouselItem {
  name: string;
  text: JSX.Element;
}

interface ICarouselProps {
  items: ICarouselItem[];
}

interface ICarouselState {
  selected: number;
}

import renderComponentIf from "../global/renderComponentIf";

const startProjectSteps: Array<ICarouselItem> = [
  {
    name: "Register",
    text: <div>Complete the Start Project registration form.</div>
  },
  {
    name: "Assessment",
    text: <div>CollAction judges if your project meets <mark>the criteria</mark> on this page. If necessary, CollAction contacts you to discuss/clarify.</div>
  },
  {
    name: "Placement",
    text: <div>Once your idea gets the thumbs up, <mark>it will go live</mark> on collaction.org </div>
  },
  {
    name: "Campaign",
    text: <div><mark>Run a campaign</mark> to reach your target.</div>
  },
  {
    name: "Action",
    text: <div>If the target is met at the time of the deadline, <mark>all supporters will take action.</mark></div>
  },
  {
    name: "Measuring impact",
    text: <div>After the action period, <mark>measure how many people took part</mark> in the project and <mark>share this with the CollAction team.</mark></div>
  },
];


const Item = (props) => {
  return (
    <li onClick={props.onClick} className={props.isSelected ? "selected" : ""}>
       <div className="coll-action-dot-point">{props.stepCount}</div>
       {props.itemTitle}
    </li>
  );
}

class LandscapeCarousel extends React.Component<ICarouselProps, ICarouselState> {

  constructor(props) {
    super(props);
    this.state = { selected: 0 };
  }

  onItemClicked(index) {
    this.setState({selected: index});
  }

  render () {
    const currentImageSoruce: string = `/images/steps/${this.state.selected + 1}.png`;
    return (
      <div>

        <div className="row">
            <div className="col-xs-12">
                <h1>Starting a Project</h1>
                <h4>(Is Super Easy)</h4>
            </div>
        </div>

        <div className="row">
          <div className="col-md-4 col-md-offset-2 col-xs-12 landscape-carousel-select">
            <p>In 6 easy Steps</p>
          </div>

          <div className="col-xs-4 landscape-carousel-body">
            <p>{this.props.items[this.state.selected].text}</p>
          </div>
        </div>

        <div className="row">
          <div className="col-md-4 col-md-offset-2 col-xs-12 landscape-carousel-select">
            <ul>
              {this.props.items.map((item, index) => <Item key={index + 1}
                isSelected={index === this.state.selected}
                stepCount={index + 1}
                onClick={() => this.onItemClicked(index)}
                itemTitle={item.name} />)}
            </ul>
          </div>

          <div className="col-xs-4 landscape-carousel-body">
            <img src={currentImageSoruce} />
          </div>
        </div>

      </div>
    );
  }
}

interface IDropDownProps {
  label: string;
  imageSrc: string;
  index: number;
  text: JSX.Element;
}

interface IDropDownState {
  open: boolean;
}

class DropDownWithImage extends React.Component<IDropDownProps, IDropDownState> {

  constructor(props) {
    super(props);
    this.state = { open: false };
  }

  onClick () {
    this.setState({ open: !this.state.open });
  }

  renderIcon () {
    if (this.state.open) {
      return <i className="fa fa-chevron-up"></i>;
    }
    return <i className="fa fa-chevron-down"></i>;
  }

  renderContent () {
    if (!this.state.open) {
      return null;
    }

    return (
      <div className="row">
        <div className="col-xs-8 col-xs-offset-2">
          <img src={this.props.imageSrc} />
        </div>
        <div className="col-xs-8 col-xs-offset-2 carousel-text">
          { this.props.text }
        </div>
      </div>
    );
  }

  render () {
    return (
      <div onClick={() => this.onClick()} >
        <div className="drop-down row">
          <span>{this.props.index}</span> {this.props.label} {this.renderIcon()}
        </div>
        { this.renderContent() }
      </div>
    );
  }
}

class MobileCarousel extends React.Component<ICarouselProps, ICarouselState> {

  imageNameForIndex (index) {
    return `/images/steps/${index + 1}.png`;
  }

  render () {
    return (
      <div id="mobile-carousel">
          <div className="col-xs-12">
              {this.props.items.map((item, index) => <DropDownWithImage
                key={index + 1}
                label={item.name}
                text={item.text}
                index={index + 1}
                imageSrc={this.imageNameForIndex(index)} />)}
          </div>
      </div>
    );
  }
}

renderComponentIf(
  <LandscapeCarousel items={startProjectSteps} />,
  document.getElementById("project-create-landscape-carousel")
);

renderComponentIf(
  <MobileCarousel items={startProjectSteps} />,
  document.getElementById("project-create-mobile-carousel")
);
