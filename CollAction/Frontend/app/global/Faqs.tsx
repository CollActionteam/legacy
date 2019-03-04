import * as React from "react";
import Collapsible from "react-collapsible";

import renderComponentIf from "./renderComponentIf";

interface IFaqProps {
    title: any;
    content: any;
}

export default class Faq extends React.Component<IFaqProps> {
    constructor(props: IFaqProps) {
        super(props);
    }

    renderTitle() {
        return (
            <h3>
                <a href="#">{this.props.title}</a>
                <span className="toggle">
                    <i className="fa fa-plus"></i>
                    <i className="fa fa-minus"></i>
                </span>
            </h3>
        );
    }

    render() {
        return (
            <Collapsible trigger={this.renderTitle()} transitionTime={300} lazyRender={true}>
                {this.props.content}
            </Collapsible>
        );
    }
}