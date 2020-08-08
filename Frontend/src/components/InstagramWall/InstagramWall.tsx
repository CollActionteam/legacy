import React from "react";

import styles from "./InstagramWall.module.scss";
import Formatter from "../../formatter";
import { IInstagramWallItem } from "../../api/types";
import LazyImage from "../LazyImage/LazyImage";


export interface IInstagramWallProps {
    wallItems: IInstagramWallItem[];
}

const getProxiedUrl = (url: string) =>
    `${process.env.REACT_APP_BACKEND_URL}/proxy?url=${encodeURIComponent(url)}`;

export default (props: IInstagramWallProps) => (
    <div className={styles.container}>
        {
            props.wallItems.map(item => 
                (<a href={item.link} target="_blank" rel="noopener noreferrer" key={"iw" + item.shortCode}>
                    <LazyImage src={getProxiedUrl(item.thumbnailSrc)} alt={item.accessibilityCaption ?? undefined}/>
                    <div className={styles.caption}>
                        <div className={styles.padding}>{item.caption}</div>
                    </div>
                    <div className={styles.date}>{Formatter.date(new Date(item.date))}</div>
                </a>))
        }
    </div>
);
