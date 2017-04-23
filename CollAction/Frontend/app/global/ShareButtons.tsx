
import * as React from "react";
import { ShareButtons as ReactShareButtons, ShareCounts } from "react-share";
import renderComponentIf from "./renderComponentIf";
import "./share.scss";

const {
  FacebookShareButton,
  TwitterShareButton,
  LinkedinShareButton,
} = ReactShareButtons;

const {
  FacebookShareCount,
  TwitterShareCount,
  LinkedinShareCount,
} = ShareCounts;

interface IShareButtonsProps {
  title?: string;
  url?: string;
}

interface IShareButtonsState {
  count: number;
}

/**
 * TODO : Currently this component uses the react share library to extract values that have been
 * intended to rendered. To do this we have had to code a little hack to add a number to the total share count
 * that will pull the count from the component and add it to the state. The result of this is that updating the
 * state causes a rerender so we have added our second hack to stop this component looping.
 *
 * Ideally here we should call the API's directly. We can update this post launch.
 */
class ShareButtons extends React.Component<IShareButtonsProps, IShareButtonsState> {

  constructor (props) {
    super(props);
    this.state = { count: 0 };
  }

  shouldComponentUpdate(nextProps: IShareButtonsProps, nextState:IShareButtonsState) {
    return !(nextState.count === this.state.count);
  }

  addToShareCount(amount: number) {
    console.log(amount);
    this.setState({
      count: this.state.count + amount,
    });
  }

  renderShareCounts(url: string) {
    const self = this;
    return (
      <div style={{display: "none"}}>
        <FacebookShareCount url={url}>
          {share => self.addToShareCount(share)}
        </FacebookShareCount>
        <LinkedinShareCount url={url} >
          {share => self.addToShareCount(share)}
        </LinkedinShareCount>
      </div>
    );
  }

  render () {
    const url: string = window.location.host + window.location.pathname;
    const title: string = this.props.title || window.document.title;
    return (
      <div className="share-buttons">
        {this.renderShareCounts(url)}
        <div className="row">
          <div className="col-xs-3 share-count">
            {this.state.count}<br /> Shares
          </div>
          <div className="col-xs-3">
            <FacebookShareButton title={title} url={url} >
              <i className="fa fa-facebook"></i>
            </FacebookShareButton>
          </div>
          <div className="col-xs-3">
            <TwitterShareButton url={url} title={title}>
              <i className="fa fa-twitter"></i>
            </TwitterShareButton>
          </div>
          <div className="col-xs-3">
            <LinkedinShareButton url={url} title={title} description={url}>
              <i className="fa fa-linkedin"></i>
            </LinkedinShareButton>
          </div>
        </div>
      </div>
    );
  }
}

renderComponentIf(
  <ShareButtons />,
  document.getElementById("homepage-share-buttons")
);

renderComponentIf(
  <ShareButtons />,
  document.getElementById("project-details-share-buttons")
);