import React, {useState} from "react";

import styles from "./InstagramWall.module.scss";
import Formatter from "../../formatter";
import moment from "moment";


export interface IInstagramWallProps {
    user: string;
}

export class InstagramWall extends React.Component<IInstagramWallProps> {
    constructor(props: Readonly<IInstagramWallProps>) {
        super(props);
        this.state = {items: []}
    }

    //@todo: thumbnail quality
    //@todo: video's: is_video
    //@todo: check faulty returns

    componentDidMount() {

        let url = "https://www.instagram.com/" + this.props.user + "/?__a=1";
        let _this = this;

        fetch(url).then(res => res.json())
            .then(res => {
                _this.setState({
                    items: res.graphql.user.edge_owner_to_timeline_media.edges.map((edge: { node: any; }) => {
                        let {
                            shortcode,
                            thumbnail_src,
                            accessibility_caption,
                            edge_media_to_caption,
                            taken_at_timestamp,
                        } = edge.node;
                        return {
                            shortcode,
                            thumbnail_src,
                            accessibility_caption,
                            date: Formatter.date(moment.unix(taken_at_timestamp).toDate()),
                            caption: edge_media_to_caption &&
                                edge_media_to_caption.edges &&
                                edge_media_to_caption.edges[0] &&
                                edge_media_to_caption.edges[0].node &&
                                edge_media_to_caption.edges[0].node.text,
                            link: "https://www.instagram.com/p/" + shortcode,
                        }
                    })
                });
            })
    }


    render() {
        return <div className={styles.container}>
            {
                // @ts-ignore
                this.state.items.map(item => <a href={item.link} target="_blank" rel="noopener noreferrer" key={"iw" + item.shortcode}>
                    <img src={item.thumbnail_src} alt={item.accessibility_caption}/>
                    <div className={styles.caption}>
                        <div className={styles.padding}>{item.caption}</div>
                    </div>
                    <div className={styles.date}>{item.date}</div>
                </a>)}
        </div>;
    }
}
