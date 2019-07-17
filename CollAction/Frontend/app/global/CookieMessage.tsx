import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface ICookieMessageProps {
    showCookieDialog: boolean;
    cookieAccept: string;
    cookieReject: string;
}

interface ICookieMessageState {
    showMoreOptions: boolean;
    analyticsCookieChecked: boolean;
}

export default class CookieMessage extends React.Component<ICookieMessageProps, ICookieMessageState> {
    constructor(props) {
        super(props);
        this.state = {
            showMoreOptions: false,
            analyticsCookieChecked: false
        };
    }

    accept() {
        if (!this.state.showMoreOptions || this.state.analyticsCookieChecked) {
            document.cookie = this.props.cookieAccept;
        }
        else {
            document.cookie = this.props.cookieReject;
        }
        location.reload();
    }

    changeOptions() {
        this.setState({
            showMoreOptions: !this.state.showMoreOptions
        });
    }

    toggleAnalytics() {
        this.setState({
            analyticsCookieChecked: !this.state.analyticsCookieChecked
        })
    }

    renderButtons() {
        return <React.Fragment>
            <p><a href="#" className="btn btn-default" onClick={() => this.accept()}>Accept</a></p >
            <p><a href="#" className="btn btn-default" onClick={() => this.changeOptions()}>{this.state.showMoreOptions ? "Less options" : "More options"}</a></p>
        </React.Fragment>;
    }

    renderText() {
        return <p>
            Thank you for visiting our website!
            Please note that our website uses cookies to analyze and improve the performance of our website and to make social media integration possible.
            For more information on the use of cookies and privacy related matters, please see our <a href="/home/privacy">Privacy and Cookies Policy</a>.
            By clicking "Accept", you consent to the use of cookies, analytics and social-media integration.
            Click on "More options" to customize your choice.
            If you want to change your choice later, please visit our privacy agreement.
        </p>;
    }

    renderMoreOptions() {
        if (this.state.showMoreOptions) {
            return <div className="row">
                <div className="col-xs-12">
                    {this.renderText()}
                    <div className="checkbox">
                        <input type="checkbox" name="essential-cookies" id="essential-cookies" checked readOnly disabled />
                        <label htmlFor="essential-cookies">
                            Essential cookies &amp; anonymous statistics
                        </label>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" name="integrations-analytics" id="integrations-analytics" checked={this.state.analyticsCookieChecked} onClick={() => this.toggleAnalytics()} />
                        <label htmlFor="integration-analytics" onClick={() => this.toggleAnalytics()}>
                            External integrations (youtube, stripe, facebook, ..) and analytics
                        </label>
                    </div>
                    {this.renderButtons()}
                </div>
            </div>;
        }
    }

    renderStandardOptions() {
        if (!this.state.showMoreOptions) {
            return <div className="container">
                <div className="row">
                    <div className="col-xs-12">
                        {this.renderText()}
                        {this.renderButtons()}
                    </div>
                </div>
            </div>;
        }
    }

    render() {
        if (this.props.showCookieDialog) {
            return <div>
                <div className="cookie-message-banner">
                    {this.renderStandardOptions()}
                    {this.renderMoreOptions()}
                </div>
            </div>;
        }
        else {
            return <div></div>;
        }
    }
}

["cookie-message", "cookie-message-change"].forEach(function(dialogId) {
    renderComponentIf(
        <CookieMessage
            showCookieDialog={document.getElementById(dialogId) && document.getElementById(dialogId).dataset.showCookieDialog.toLowerCase() === "true"}
            cookieAccept={document.getElementById(dialogId) && document.getElementById(dialogId).dataset.cookieAccept}
            cookieReject={document.getElementById(dialogId) && document.getElementById(dialogId).dataset.cookieReject}
        />,
        document.getElementById(dialogId)
    );
});