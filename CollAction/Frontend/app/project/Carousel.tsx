import * as React from "react";
import * as ReactDOM from "react-dom";

interface ICarouselItem {
  name: string;
  text: JSX.Element;
  imageOverlay?: string;
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
    name: "Aanmelden",
    text: <div>Vul het <a target="_blank" href="/Projects/Create">Start Project formuler</a> in.</div>,
    imageOverlay: "Aanmelden",
  },
  {
    name: "Beoordeling",
    text: <div>Freonen en CollAction beoordelen of je project voldoet aan de <a href="/About#faq-criteria">criteria</a>. Indien nodig, neemt CollAction contact met je op om het te bespreken/verduidelijken.</div>
  },
  {
    name: "Plaatsing",
    text: <div>Het project wordt bij goedkeuring <mark>geplaatst</mark> op freonen.collaction.org.</div>
  },
  {
    name: "Campagne",
    text: <div><mark>Voer campagne</mark> om jouw target te bereiken.</div>
  },
  {
    name: "Actie",
    text: <div>Wanneer het target is bereikt voor de deadline, <mark>komen alle supporters in actie</mark>.</div>
  },
  {
    name: "Impactmeting",
    text: <div><mark>Meet</mark> na de actieperiode hoeveel mensen uiteindelijk hebben gehandeld en <mark>deel dit met Freonen</mark>.</div>
  },
];


const Item = (props) => {
  return (
    <li onClick={props.onClick} className={props.isSelected ? "selected" : ""}>
       <div className="coll-action-dot-point">{props.stepCount}</div>
       {props.itemTitle}
    </li>
  );
};

class LandscapeCarousel extends React.Component<ICarouselProps, ICarouselState> {

  constructor(props) {
    super(props);
    this.state = { selected: 0 };
  }

  onItemClicked(index) {
    this.setState({selected: index});
  }

  renderImageOverlay () {
    const imageOverlay: string = this.props.items[this.state.selected].imageOverlay;
    if (!imageOverlay) {
      return null;
    }

    return <div className="image-overlay">{ imageOverlay }</div>;
  }

  render () {
    const currentImageSoruce: string = `/images/friesland/steps/${this.state.selected + 1}.png`;
    return (
      <div>

        <div className="row">
            <div className="col-xs-12">
                <h1>Een project starten</h1>
                <h4>(is simpel)</h4>
            </div>
        </div>

        <div className="row">
          <div className="col-md-4 col-md-offset-2 col-xs-6 landscape-carousel-select">
            <p>In 6 stappen:</p>
          </div>

          <div className="col-xs-6 landscape-carousel-body">
            <p>{this.props.items[this.state.selected].text}</p>
          </div>
        </div>

        <div className="row">
          <div className="col-md-4 col-md-offset-2 col-xs-6 landscape-carousel-select">
            <ul>
              {this.props.items.map((item, index) => <Item key={index + 1}
                isSelected={index === this.state.selected}
                stepCount={index + 1}
                onClick={() => this.onItemClicked(index)}
                imageOverlay={item.imageOverlay}
                itemTitle={item.name} />)}
            </ul>
          </div>

          <div className="col-xs-6 landscape-carousel-body">
            <img src={currentImageSoruce} />
            {this.renderImageOverlay()}          
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
    return `/images/friesland/steps/${index + 1}.png`;
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
