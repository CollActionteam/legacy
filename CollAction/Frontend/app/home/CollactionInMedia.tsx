import * as React from "react";
import renderComponentIf from "../global/renderComponentIf";


let Carousel = require("react-responsive-carousel").Carousel;

interface IMediaAppearance {
    name: string;
    url: string;
    photo: string;
  }
interface IMediaApperanceProps {
    apperances: IMediaAppearance[];
  }
interface IMediaApperanceState {
    currentIndex: number;
  }

const ApperanceItem = ({ apperance }) => {
    const imageSrc = `/images/team/${apperance.photo}`;
    return (
    <div>
        <img src={imageSrc} />
        <p className="legend">{apperance.name}</p>
    </div>
    );
};

export default class MediaApperances extends React.Component<IMediaApperanceProps, IMediaApperanceState> {

    constructor (props) {
      super(props);
      this.state = { currentIndex: 0 };
    }
    render () {
      return (
        <div>
          <div id="media-apperances-container">
            <Carousel showArrows={true}>
                <div>
                    <img src="/images/team/NoPhoto.png" />
                    <p className="legend">Legend 1</p>
                </div>
                <div>
                    <img src="/images/team/NoPhoto.png" />
                    <p className="legend">Legend 2</p>
                </div>
                {/* {this.props.apperances.map(apperance => <ApperanceItem key={apperance.name} apperance={apperance} />)} */}
            </Carousel>
          </div>
        </div>
      );
    }
  }

const mediaApperances: IMediaAppearance[] = [
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
    },
    {
        name: "media1",
        url: "Url",
        photo: "NoPhoto.png"
      }
    ];

renderComponentIf(
    <MediaApperances apperances={mediaApperances} />,
    document.getElementById("collaction-media-widget")
  );