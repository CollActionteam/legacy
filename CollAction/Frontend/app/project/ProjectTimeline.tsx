import * as React from "react";
import renderComponentIf from "../global/renderComponentIf";

interface IProjectTimelineProps {

}

interface IProjectTimelineState {

}

class ProjectTimeline extends React.Component<IProjectTimelineProps, IProjectTimelineState> {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="project-timeline">
                <h1>Timeline</h1>
                <ProjectTimelineComment></ProjectTimelineComment>
            </div>
        );
    }

    componentDidMount() {
    }

    componentDidUpdate() {
    }
}

class ProjectTimelineComment extends React.Component {
    render() {
        return (
            <div className="timeline-comment">
                <div className="timeline-comment__author">
                    <div className="timeline-comment__date">
                        19 september
                    </div>
                    <div className="timeline-comment__avatar">
                        <span className="timeline-comment__avatar-fallback">IN</span>
                        <img className="timeline-comment__avatar-image" src="" />
                    </div>
                </div>
                <div className="timeline-comment__content">
                    <h3>Author name</h3>
                    <div className="timeline-comment__project-starter-label">
                        Project starter
                    </div>
                    <div>
                        Text
                    </div>
                    <div className="timeline-comment__options">
                        <div>Like</div>
                        <div>4 Comments</div>
                        <div>Reply</div>
                    </div>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <ProjectTimeline />,
    document.getElementById("project-timeline")
);