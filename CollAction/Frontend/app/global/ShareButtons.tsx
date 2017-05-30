import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "./renderComponentIf";
import registerGlobal from "./registerGlobal";

enum States { ERROR, READY, LOADING };

interface IShareButtonsProps {
  title?: string;
  url?: string;
}

interface IShareButtonsState {
  shareCount?: number;
  shareCountError?: Error;
  shareCountState: States;
}

class ShareButtons extends React.Component<IShareButtonsProps, IShareButtonsState> {

  constructor(props) {
    super(props);
    this.state = { shareCountState: States.LOADING };
  }

  componentDidMount() {
    this.getShareCount();
  }

  getTitle() {
    return this.props.title || document.title;
  }

  getUrl() {
    return this.props.url || window.location;
  }

  async getLinkedInShareCount() {
    try {
      const apiLink: string = `https://www.linkedin.com/countserv/count/share?url=${this.getUrl()}&format=json`;
      const response: Response = await fetch(apiLink);
      const parsed = await response.json();
      return parsed.count;
    } catch (e) {
      return 0;
    }
  }

  async getFacebookShareCount() {
    const apiLink: string = `http://graph.facebook.com/?id=${this.getUrl()}`;
    const response: Response = await fetch(apiLink);
    const parsed = await response.json();
    return parsed.share.share_count;
  }

  async getShareCount() {
    try {
      const shareCount: number = await this.getFacebookShareCount();
      this.setState({ shareCount, shareCountState: States.READY });
    } catch (e) {
      this.setState({ shareCountError: e, shareCountState: States.ERROR });
    }
  }

  getTwitterUrl () {
    return encodeURI(`https://twitter.com/intent/tweet?text=${this.getTitle()}&url=${this.getUrl()}`);
  }

  getFacebookUrl() {
    return encodeURI(`https://www.facebook.com/sharer/sharer.php?u=${this.getUrl()}`);
  }

  getLinkedInUrl () {
    return encodeURI(`http://www.linkedin.com/shareArticle?mini=true&url=${this.getUrl()}L&title=${this.getTitle()}`);
  }



  render() {
    return (
      <div className="share-buttons">
        <div className="row">
          <div className="col-xs-3 share-count">
            {this.state.shareCount}<br /> Shares
          </div>
          <div className="col-xs-3">
            <a href={this.getFacebookUrl()} target="_blank">
              <div className="social-media-share-buttons social-media-share-button-facebook">
                <i className="fa fa-facebook"></i>
              </div>
            </a>
          </div>
          <div className="col-xs-3">
            <a href={this.getTwitterUrl()}>
              <div className="social-media-share-buttons social-media-share-button-twitter">
                <i className="fa fa-twitter"></i>
              </div>
            </a>
          </div>
          <div className="col-xs-3">
            <a href={this.getLinkedInUrl()}>
              <div className="social-media-share-buttons social-media-share-button-linkedin">
                <i className="fa fa-linkedin"></i>
              </div>
            </a>
          </div>
        </div>
      </div>
    );
  }
}

const FullShareButtons = () => {
  return (
    <div className="row">
      <div className="col-xs-12 col-md-8 col-md-offset-2 share-container">
        <h3 className="share-title">Spread it further</h3>
        <div className="row">
          <div className="col-xs-12 col-sm-6 col-sm-offset-3 share-buttons-container">
            <ShareButtons />
          </div>
        </div>
      </div>
    </div>
  );
};

renderComponentIf(
  <FullShareButtons />,
  document.getElementById("homepage-share-buttons")
);

renderComponentIf(
  <ShareButtons />,
  document.getElementById("project-details-share-buttons")
);

renderComponentIf(
  <FullShareButtons />,
  document.getElementById("project-details-share-buttons-row")
);

function renderShareWithTitleAndLink(title: string, url: string, element: HTMLElement) {
  ReactDOM.render(<ShareButtons title={title} url={url} />, element);
}

registerGlobal("renderShareWithTitleAndLink", renderShareWithTitleAndLink);
