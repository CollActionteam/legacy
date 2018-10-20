import * as React from "react";
import Tooltip from "rc-tooltip";
import renderComponentIf from "./renderComponentIf";
import "rc-tooltip/assets/bootstrap_white.css";

renderComponentIf(
    <Tooltip
        placement="topLeft"
        align={{
            offset: [-30, -10]
        }}
        arrowContent={<div className="rc-tooltip-arrow-inner"></div>}
        overlay={
            <span>
                Please choose your target<br/>
                number of participants here.<br/>
                In other words: what is the<br/>
                minimum amount of people that <br/>
                have to commit to taking the action,<br/>
                for the crowdact to take place?<br/>
                Read more about how to choose a<br/>
                target <a href="/about#faq" target="_blank">here</a>.
            </span>}>
        <i className="fa fa-question-circle-o"></i>
    </Tooltip>,
    document.getElementById("target-tooltip")
);

renderComponentIf(
    <Tooltip
        placement="topLeft"
        align={{
            offset: [-25, -10]
        }}
        arrowContent={<div className="rc-tooltip-arrow-inner"></div>}
        overlay={
            <span>
                In one short sentence:<br/>
                what is the action that<br/>
                you're proposing and how<br/>
                many people have to commit<br/>
                for the collective action<br/>
                to take place?<br/>
                E.g. use the format:<br/>
                "If X people commit to do Y,<br/>
                then we'll all do it together!"
            </span>}>
        <i className="fa fa-question-circle-o"></i>
    </Tooltip>,
    document.getElementById("proposal-tooltip")
);