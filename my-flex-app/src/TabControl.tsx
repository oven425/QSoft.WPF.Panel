import type { ReactElement } from "react";
import React from "react";

interface TabControlProps {
  children: ReactElement<typeof TabItem> | ReactElement<typeof TabItem>[];
}
export const TabControl = ({ children }: TabControlProps)=>{
    const validChildren = React.Children.toArray(children).filter((child) => {
    return React.isValidElement(child) && child.type === TabItem;
  }) as ReactElement[];
    return(
        // <div className="flex">
        //     {validChildren.map((child, index) => (
        //   <button key={index} className="px-4 py-2 border-r">
        //     {child.props.header}
        //   </button>
        // ))}
        // </div>
    )
}

export const TabItem = ()=>{
    return(
        <div>

        </div>
    )
}

const Test = ()=>{
    return(
        <TabControl>
            <TabItem>

            </TabItem>
        </TabControl>
    )
}