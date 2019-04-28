import * as React from "react";
import renderComponentIf from "../global/renderComponentIf";

export type ProjectComment = {
    date: Date;
    author: string;
    avatarUrl?: string;
    text: string;
};

interface IProjectTimelineProps {
}

interface IProjectTimelineState {
    comments: ProjectComment[];
    isLoading: boolean;
}

interface IProjectTimelineCommentProps {
    comment: ProjectComment;
    subComments?: ProjectComment[];
}

interface IProjectTimelineCommentState {
    isEditing: boolean;
}

class ProjectTimeline extends React.Component<IProjectTimelineProps, IProjectTimelineState> {
    constructor(props) {
        super(props);

        this.state = {
            comments: [],
            isLoading: false
        };
    }

    render() {
        return (
            <div className="project-timeline">
                <h1>Timeline</h1>
                <ul className="timeline-comment-list">
                    {this.state.comments.map(comment => <ProjectTimelineComment comment={comment}></ProjectTimelineComment>) }
                </ul>
            </div>
        );
    }

    componentDidMount() {
        /**
         * Dummy data
        */
       const dummyComments: ProjectComment[] = [
            {
                date: new Date(),
                author: "Arnout Jansen",
                text: "This is a test comment",
                avatarUrl: "http://i.pravatar.cc/300"
            },
            {
                date: new Date(),
                author: "Ron van den Akker",
                text: "This is a second test comment",
                avatarUrl: "http://i.pravatar.cc/300"
            },
            {
                date: new Date(),
                author: "Daniela Becker",
                text: "This is a third test comment",
                avatarUrl: "http://i.pravatar.cc/300"
            }
        ];

        /**
         * Set dummy comments
         */
        this.setState(({ comments: dummyComments }));
    }

    componentDidUpdate() {
    }
}

class ProjectTimelineComment extends React.Component<IProjectTimelineCommentProps, IProjectTimelineCommentState> {
    constructor(props) {
        super(props);

        this.state = {
            isEditing: false
        };
    }

    render() {
        return (
            <li className="timeline-comment">
                <div className="timeline-comment__author">
                    <div className="timeline-comment__date">
                        {this.props.comment.date.toLocaleDateString()}
                    </div>
                    <div className="timeline-comment__avatar">
                        <span className="timeline-comment__avatar-fallback">I.N.</span>
                        { this.props.comment.avatarUrl
                        ? <img className="timeline-comment__avatar-image" src={this.props.comment.avatarUrl} />
                        : null
                        }
                    </div>
                </div>
                <div className="timeline-comment__content">
                    <h3 className="timeline-comment__title">
                        {this.props.comment.author}
                    </h3>
                    <div className="timeline-comment__project-starter-label">
                        Project starter
                    </div>
                    <div>
                        {this.props.comment.text}
                    </div>
                    <div className="timeline-comment__options">
                        <button className="timeline-comment__button">
                            Like
                        </button>
                        { this.props.subComments && this.props.subComments.length
                            ? <div>
                                { this.props.subComments.length } replies
                            </div>
                            : null
                        }
                        <button className="timeline-comment__button">
                            Reply
                        </button>
                    </div>
                </div>
            </li>
        );
    }
}

renderComponentIf(
    <ProjectTimeline />,
    document.getElementById("project-timeline")
);